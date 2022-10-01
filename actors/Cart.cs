using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Cart : Spatial
{
    // carts follow a series of steps
    // carts take 1 move per 0.5 seconds

    public const float CART_MOVE_TIME = 0.5f;
    public const int CART_MAX_TICKS = 14;

    public int StartTick;

    public int ID;

    public Recipe Recipe;

    public CartState CurrentCartState;

    public Dictionary<int, CartAction> PlannedActions = new Dictionary<int, CartAction>();

    public IntVec2 ExitPoint => new IntVec2(11, 4);

    public override void _Ready()
    {
        Recipe = new Recipe
        {
            Ings = new Recipe.Ing[]{
                Recipe.Ing.Lettuce
            }
        };

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
        foreach (var it in GetTree().Root.FindChildrenByType<Cart>())
        {
            cgs.CartStates[it.ID] = it.CurrentCartState;
        }
        foreach (var it in GetTree().Root.FindChildrenByType<Station>())
        {
            cgs.StationStates[it.ID] = it.StationState;
        }


        var tgs = new GameState();
        foreach (var it in GetTree().Root.FindChildrenByType<Cart>())
        {
            tgs.CartStates[it.ID] = it.CurrentCartState;
        }
        foreach (var it in GetTree().Root.FindChildrenByType<Station>())
        {
            tgs.StationStates[it.ID] = it.StationState;
        }

        var ocs = tgs.CartStates[ID];
        ocs.Pos = new IntVec2(11, 4);
        ocs.Ings = Recipe.Ings.ToList();
        tgs.CartStates[ID] = ocs;

        var aStar = new AStarIndexed<AStarNode>(new CartModel(this));

        var nodes = aStar.FindPath(
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
            maxIteration: 50_000
        );

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                GD.Print($"{node.GameState.CurrentTick - 1} - {node.MyAction}");
                PlannedActions[node.GameState.CurrentTick - 1] = node.MyAction;
            }
        }
    }


    public override void _Process(float delta)
    {
        var def = GetTree().Root.FindChildByType<Default>();
        var ct = def.CurrentTick;
        if (PlannedActions.ContainsKey(ct))
        {
            PlannedActions[ct].Execute(def);

            this.SetGlobalLocation(new Vector3(CurrentCartState.Pos.x, 0, CurrentCartState.Pos.y));

            PlannedActions.Remove(ct);
        }
    }

    struct AStarNode : IEquatable<AStarNode>, IComparable<AStarNode>
    {
        private static ulong NextNodeID = 0;

        ulong NodeID;

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

        public CartModel(Cart cart)
        {
            this.Cart = cart;

            foreach (var it in Cart.GetTree().Root.FindChildrenByType<Station>())
            {
                foreach (var it2 in it.GetBlocked())
                {
                    BlockedMap[it2] = it;
                    GD.Print($"BlockedMap[{it2}] = {it}");
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

            //GD.Print($"{ngs.CartStates[Cart.ID].Pos} / {ngs.CartStates[Cart.ID].Facing}");

            return new AStarNode(
                ngs,
                ourAction
            );
        }

        public IEnumerable<AStarNode> GetNeighbors(AStarNode node)
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
                        if (!BlockedMap.ContainsKey(np) && (i == node.GameState.CartStates[Cart.ID].Facing || node.GameState.CartStates[Cart.ID].TurnsLeft > 0) && np.x >= 0 && np.y >= 0 && np.x < Ground.WIDTH && np.y < Ground.HEIGHT)
                        {
                            yield return Advance(node, new CAMove { CartID = Cart.ID, Dest = np, Facing = i });
                        }
                    }

                    if (!Enumerable.SequenceEqual(node.GameState.CartStates[Cart.ID].Ings, Cart.Recipe.Ings))
                    {
                        for (var i = 0; i < 4; ++i)
                        {
                            var np = node.GameState.CartStates[Cart.ID].Pos + deltas[i];
                            if (BlockedMap.ContainsKey(np))
                            {
                                //GD.Print("TRYING!");
                                yield return Advance(node, new CAUseStation { CartID = Cart.ID, StationID = BlockedMap[np].ID });
                            }
                        }
                    }
                }
            }
        }

        public uint GetMoveCostBetweenNodes(AStarNode node1, AStarNode node2)
        {

            var cs1 = node1.GameState.CartStates[Cart.ID];
            var cs2 = node2.GameState.CartStates[Cart.ID];

            if (Enumerable.SequenceEqual(cs1.Ings, cs2.Ings))
            {
                return (uint)Cart.ExitPoint.ManhattanDistanceTo(cs1.Pos);
            }

            return 100;
        }

        public ulong EstimateCostBetweenNodes(AStarNode node1, AStarNode node2)
        {
            ulong matchingIngs = 0;
            var cs1 = node1.GameState.CartStates[Cart.ID];
            var cs2 = node2.GameState.CartStates[Cart.ID];

            if (cs1.Ings.Count > 0)
            {
                //GD.Print("GREATER");
                AT.True(cs1.Ings[0] == Recipe.Ing.Lettuce);
            }

            AT.Eq(cs2.Ings.Count, 1);
            AT.True(cs2.Ings[0] == Recipe.Ing.Lettuce);

            for (var i = 0; i < 10; i++)
            {
                if (i >= cs1.Ings.Count) continue;
                if (i >= cs2.Ings.Count) continue;

                if (cs1.Ings[i] == cs2.Ings[i]) matchingIngs++;
            }

            var ret = 1_000_000ul - (500ul * matchingIngs);

            //if (ret < 1_000_000ul) GD.Print($"ret={ret}");

            return ret;
        }
    }
}