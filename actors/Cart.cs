using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Cart : Spatial
{
    // carts follow a series of steps
    // carts take 1 move per 0.5 seconds

    public const float CART_MOVE_TIME = 0.5f;
    public const int CART_MAX_TICKS = 45;

    public int StartTick;

    public int ID;

    public Recipe Recipe;

    public CartState CurrentCartState;

    public Dictionary<int, CartAction> PlannedActions = new Dictionary<int, CartAction>();

    public static IntVec2 ExitPoint => new IntVec2(11, 4);
    public static IntVec2 StartPoint => new IntVec2(0, 4);

    public Vector3 PosToMoveTo;
    public float BearingToTurnTo;

    public IEnumerator<object> FindThePathEnumerator;

    public List<Spatial> StackedFood = new List<Spatial>();

    private IEnumerable<object> Prep()
    {
        Recipe = GetTree().Root.FindChildByType<Default>().GetNextRecipe();

        ID = GetTree().Root.FindChildrenByType<Cart>().Select(it => it.ID).Max() + 1;

        StartTick = GetTree().Root.FindChildByType<Default>().CurrentTick;

        CurrentCartState = new CartState
        {
            Facing = 0,
            Ings = new System.Collections.Generic.List<Recipe.Ing>(),
            Pos = new IntVec2(-10, -10),
            TurnsLeft = 5,
        };

        var cgs = new GameState();
        cgs.CurrentTick = StartTick;
        foreach (var it in GetTree().Root.FindChildrenByType<Cart>())
        {
            cgs.CartStates[it.ID] = it.CurrentCartState;
        }
        foreach (var it in GetTree().Root.FindChildrenByType<Station>().Where(it => it.Built))
        {
            cgs.StationStates[it.ID] = it.StationState;
        }


        var tgs = new GameState();
        tgs.CurrentTick = StartTick;
        foreach (var it in GetTree().Root.FindChildrenByType<Cart>())
        {
            tgs.CartStates[it.ID] = it.CurrentCartState;
        }
        foreach (var it in GetTree().Root.FindChildrenByType<Station>().Where(it => it.Built))
        {
            tgs.StationStates[it.ID] = it.StationState;
        }

        var ocs = tgs.CartStates[ID];
        ocs.Pos = new IntVec2(11, 4);
        ocs.Ings = Recipe.Ings.ToList();
        tgs.CartStates[ID] = ocs;

        var aStar = new AStarIndexed<AStarNode>(new CartModel(this));

        foreach (var it in aStar.FindPath(
            new AStarNode(cgs, null),
            new AStarNode(tgs, null),
            (it) =>
            {
                AT.NotNull(it.GameState);
                AT.Contains(it.GameState.CartStates.Keys, ID);
                AT.NotNull(it.GameState.CartStates[ID]);
                AT.NotNull(it.GameState.CartStates[ID].Ings);
                AT.NotNull(Recipe);
                AT.NotNull(Recipe.Ings);

                return Enumerable.SequenceEqual(it.GameState.CartStates[ID].Ings, Recipe.Ings) && it.GameState.CartStates[ID].Pos == ExitPoint;
            },
            maxIteration: 5_000
        ))
        {
            yield return it;
        }

        var nodes = aStar.LastPath;

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                GD.Print($"{node.GameState.CurrentTick - 1} - {node.NodeID} - {node.MyAction}");
                PlannedActions[node.GameState.CurrentTick - 1] = node.MyAction;
            }
        }
        else
        {
            GD.PushWarning("NO result found!");
        }
    }

    public override void _Ready()
    {
        this.GetTree().Root.FindChildByType<Default>().Paused = true;
        FindThePathEnumerator = Prep().GetEnumerator();
    }


    public override void _Process(float delta)
    {
        if (FindThePathEnumerator != null)
        {
            if (FindThePathEnumerator.MoveNext())
            {
                GD.Print($"Finding the path... {FindThePathEnumerator.Current}");
                return;
            }
            else
            {
                this.GetTree().Root.FindChildByType<Default>().Paused = false;
                FindThePathEnumerator = null;
            }
        }

        var def = GetTree().Root.FindChildByType<Default>();
        var ct = def.CurrentTick;
        if (PlannedActions.ContainsKey(ct) && PlannedActions[ct] != null)
        {
            PlannedActions[ct].Execute(def, true);

            PosToMoveTo = new Vector3(CurrentCartState.Pos.x, 0, CurrentCartState.Pos.y);
            BearingToTurnTo = (3 - CurrentCartState.Facing) * Mathf.Pi / 2;

            PlannedActions.Remove(ct);
        }

        var startRot = this.GlobalTransform.basis.RotationQuat();
        var targetRot = new Quat(new Vector3(0, BearingToTurnTo, 0));

        if (startRot.AngleTo(targetRot) > 0.02f)
        {
            GlobalTransform = new Transform(
                startRot.Slerp(targetRot, 0.52f),
                GlobalTransform.origin
            );
        }
        else
        {
            var speed = 1 / Cart.CART_MOVE_TIME * delta * 1.5f;
            var deltaPos = PosToMoveTo - this.GetGlobalLocation();
            if (deltaPos.Length() < speed || deltaPos.Length() > 1.5f)
            {
                this.SetGlobalLocation(PosToMoveTo);
            }
            else
            {
                this.SetGlobalLocation(this.GetGlobalLocation() + deltaPos.Normalized() * speed);
            }
        }

        if (StackedFood.Count < CurrentCartState.Ings.Count)
        {
            var foodToAddType = CurrentCartState.Ings[StackedFood.Count];
            AT.True(foodToAddType != Recipe.Ing.None);
            AT.Contains(IngModels.Data.Keys, foodToAddType);
            var nextFoodData = IngModels.Data[foodToAddType];

            var nf = GD.Load<PackedScene>(nextFoodData.Path).Instance<Spatial>();
            nf.Translation = new Vector3(0, nextFoodData.Height, 0);
            if (StackedFood.Count > 0)
                StackedFood.Last().AddChild(nf);
            else
                AddChild(nf);

            StackedFood.Add(nf);
        }
    }

    struct AStarNode : IEquatable<AStarNode>, IComparable<AStarNode>
    {
        private static ulong NextNodeID = 0;

        public ulong NodeID;

        public AStarNode(GameState gs, CartAction myAction)
        {
            NodeID = NextNodeID++;
            GameState = gs;
            MyAction = myAction;
        }

        public GameState GameState;

        // this is the action I took to _get_ to this state
        public CartAction MyAction;

        public bool Equals(AStarNode other)
        {
            return NodeID == other.NodeID;
        }

        public int CompareTo(AStarNode other)
        {
            return NodeID.CompareTo(other.NodeID);
        }
    }

    class CartModel : AStarIndexed<AStarNode>.IModel
    {
        Cart Cart;

        Dictionary<IntVec2, Station> BlockedMap = new Dictionary<IntVec2, Station>();

        Dictionary<Tuple<IntVec2, Recipe.Ing>, int> DistanceField = new Dictionary<Tuple<IntVec2, Recipe.Ing>, int>();

        public CartModel(Cart cart)
        {
            this.Cart = cart;

            var stations = Cart.GetTree().Root.FindChildrenByType<Station>().Where(it => it.Built).ToArray();

            foreach (var it in stations)
            {
                foreach (var it2 in it.GetBlocked())
                {
                    BlockedMap[it2] = it;
                    GD.Print($"BlockedMap[{it2}] = {it}");
                }
            }

            foreach (var ing in Util.GetEnumValues<Recipe.Ing>())
            {
                var stationsOfThisType = stations.Where(it => it.IngredientDelivered == ing).ToArray();
                if (stationsOfThisType.Length > 0)
                {
                    for (var x = 0; x < Ground.WIDTH; ++x)
                    {
                        for (var y = 0; y < Ground.HEIGHT; ++y)
                        {

                            var minDist = stationsOfThisType.Select(it => it.IntPos.ManhattanDistanceTo(new IntVec2(x, y))).Min();
                            DistanceField[Tuple.Create(new IntVec2(x, y), ing)] = minDist;
                        }
                    }
                }
            }
        }

        public AStarNode Advance(AStarNode gs, CartAction ourAction)
        {
            var ngs = gs.GameState.Clone();
            ngs.CurrentTick++;

            foreach (var cartId in gs.GameState.CartStates.Keys.OrderBy(it => it))
            {
                if (cartId != Cart.ID)
                {
                    // this is a different cart
                    var otherCart = Cart.GetTree().Root.FindChildByPredicate<Cart>(it => it.ID == cartId);
                    var otherCartAction = otherCart.PlannedActions.ContainsKey(gs.GameState.CurrentTick) ? otherCart.PlannedActions[gs.GameState.CurrentTick] : null;
                    if (otherCartAction != null)
                    {
                        otherCartAction.Execute(ngs);
                    }
                }
                else
                {
                    if (ourAction != null)
                    {
                        ourAction.Execute(ngs);
                    }
                }
            }

            // if (ngs.GetCartState(Cart.ID).CanTakeNextActionAt > ngs.CurrentTick)
            // {
            //     ngs.CurrentTick = ngs.GetCartState(Cart.ID).CanTakeNextActionAt;
            // }

            //GD.Print($"{ngs.CartStates[Cart.ID].Pos} / {ngs.CartStates[Cart.ID].Facing}");

            return new AStarNode(
                ngs,
                ourAction
            );
        }

        public IEnumerable<AStarNode> GetNeighbors(AStarNode node)
        {
            if (node.GameState.CurrentTick < node.GameState.GetCartState(Cart.ID).CanTakeNextActionAt)
            {
                yield return Advance(node, null);
            }
            else
            {

                if (node.GameState.CurrentTick < Cart.StartTick + CART_MAX_TICKS)
                {
                    if (node.GameState.CartStates[Cart.ID].Pos.x < -5)
                    {
                        // we haven't entered the map yet

                        yield return Advance(node, null);
                        yield return Advance(node, new CAMove { CartID = Cart.ID, Dest = new IntVec2(0, 4), Facing = 0 });
                    }
                    else
                    {
                        var deltas = new IntVec2[]{
                            new IntVec2(1, 0),
                            new IntVec2(0, 1),
                            new IntVec2(-1, 0),
                            new IntVec2(0, -1),
                        };

                        for (var i = 0; i < 4; ++i)
                        {
                            var np = node.GameState.CartStates[Cart.ID].Pos + deltas[i];
                            if (!BlockedMap.ContainsKey(np) &&
                                (i == node.GameState.CartStates[Cart.ID].Facing || node.GameState.CartStates[Cart.ID].TurnsLeft > 0) &&
                                np.x >= 0 && np.y >= 0 && np.x < Ground.WIDTH && np.y < Ground.HEIGHT &&
                                (
                                    np == ExitPoint ||
                                    !node.GameState.CartStates.Any(it => it.Value.Pos == np)
                                )
                                )
                            {
                                yield return Advance(node, new CAMove { CartID = Cart.ID, Dest = np, Facing = i });
                            }
                        }

                        if (!Enumerable.SequenceEqual(node.GameState.CartStates[Cart.ID].Ings, Cart.Recipe.Ings))
                        {
                            var cs1 = node.GameState.CartStates[Cart.ID].Ings;
                            var recipeNeeded = Cart.Recipe.Ings;
                            var nextIngredient = cs1.Count < recipeNeeded.Length ? recipeNeeded[cs1.Count] : Recipe.Ing.None;

                            if (nextIngredient != Recipe.Ing.None)
                            {
                                for (var i = 0; i < 4; ++i)
                                {
                                    var np = node.GameState.CartStates[Cart.ID].Pos + deltas[i];
                                    if (BlockedMap.ContainsKey(np) && BlockedMap[np].IngredientDelivered == nextIngredient && node.GameState.GetStationState(BlockedMap[np].ID).CooldownWillBeUpAt <= node.GameState.CurrentTick)
                                    {
                                        //GD.Print("TRYING!");
                                        yield return Advance(node, new CAUseStation { CartID = Cart.ID, StationID = BlockedMap[np].ID });
                                    }
                                }
                            }
                            else
                            {
                                GD.PushError($"We think we're done, but {String.Join(",", node.GameState.CartStates[Cart.ID].Ings)} != {String.Join(",", Cart.Recipe.Ings)}");
                            }
                        }
                    }
                }
            }
        }

        public uint GetMoveCostBetweenNodes(AStarNode node1, AStarNode node2)
        {
            /*var cs1 = node1.GameState.CartStates[Cart.ID];
            var cs2 = node2.GameState.CartStates[Cart.ID];
            var neededCount = cs1.Ings.Count - cs2.Ings.Count;

            if (neededCount == 0)
            {
                return (uint)Cart.ExitPoint.ManhattanDistanceTo(cs1.Pos);
            }
            else
            {
                var nextIngredient = cs2.Ings[cs1.Ings.Count];
                AT.DoesNotContain(cs2.Ings, Recipe.Ing.None);
                AT.True(nextIngredient != Recipe.Ing.None);
                var tp = Tuple.Create(cs1.Pos, nextIngredient);
                if (!DistanceField.ContainsKey(tp))
                {
                    GD.PushWarning($"Can't seem to find {tp} in DistanceField, we have {cs1.Ings.Count} ingredients");
                    return 10_000;
                }

                return (uint)(neededCount * 200 + DistanceField[tp]);
            }

            return 100;*/
            return 1;
        }

        public ulong EstimateCostBetweenNodes(AStarNode node1, AStarNode node2)
        {
            //ulong matchingIngs = 0;
            var cs1 = node1.GameState.CartStates[Cart.ID];
            var cs2 = node2.GameState.CartStates[Cart.ID];
            var neededCount = cs2.Ings.Count - cs1.Ings.Count;

            if (neededCount <= 0)
            {
                //GD.Print($"{cs1.Pos} - {cs1.Ings.Count} - {Cart.ExitPoint.ManhattanDistanceTo(cs1.Pos)}");
                return (uint)Cart.ExitPoint.ManhattanDistanceTo(cs1.Pos);
            }
            else
            {
                var nextIngredient = cs2.Ings[cs1.Ings.Count];
                AT.DoesNotContain(cs2.Ings, Recipe.Ing.None);
                AT.True(nextIngredient != Recipe.Ing.None);
                var tp = Tuple.Create(cs1.Pos.x != -10 ? cs1.Pos : Cart.StartPoint, nextIngredient);
                if (!DistanceField.ContainsKey(tp))
                {
                    GD.PushWarning($"Can't seem to find {tp} in DistanceField, we have {cs1.Ings.Count} ingredients");
                    return 10_000;
                }

                //GD.Print($"{cs1.Pos} - {cs1.Ings.Count} - {neededCount * 20 + DistanceField[tp]}");

                return (uint)(neededCount * 20 + DistanceField[tp]);
            }

            /*for (var i = 0; i < 10; i++)
            {
                if (i >= cs1.Ings.Count) continue;
                if (i >= cs2.Ings.Count) continue;

                if (cs1.Ings[i] == cs2.Ings[i]) matchingIngs++;
            }

            var ret = 1_000_000ul - (500ul * matchingIngs);*/

            //if (ret < 1_000_000ul) GD.Print($"ret={ret}");

            //return ret;
        }
    }
}