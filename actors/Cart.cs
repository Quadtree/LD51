using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Cart : Spatial
{
    // carts follow a series of steps
    // carts take 1 move per 0.5 seconds

    const float CART_MOVE_TIME = 0.5f;
    const int CART_MAX_TICKS = 30;

    public int StartTick;

    public int ID;

    public Recipe Recipe;

    public CartState CurrentCartState;

    public Dictionary<int, CartAction> PlannedActions = new Dictionary<int, CartAction>();

    public override void _Ready()
    {
        ID = GetTree().Root.FindChildrenByType<Cart>().Select(it => it.ID).Max() + 1;

        StartTick = GetTree().Root.FindChildByType<Default>().CurrentTick;

        CurrentCartState = new CartState
        {
            Facing = 0,
            Ings = new System.Collections.Generic.List<Recipe.Ing>(),
            Pos = new IntVec2(-10, -10)
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

        var aStar = new AStarIndexed<AStarNode>(new CartModel(this));

        var nodes = aStar.FindPath(
            new AStarNode(cgs),
            new AStarNode(null),
            (it) => Enumerable.SequenceEqual(it.GameState.CartStates[ID].Ings, Recipe.Ings) && it.GameState.CartStates[ID].Pos == new IntVec2(11, 4)
        );

        GD.Print(nodes);
    }

    struct AStarNode : IEquatable<AStarNode>, IComparable<AStarNode>
    {
        private static ulong NextNodeID = 0;

        ulong NodeID;

        public AStarNode(GameState gs)
        {
            NodeID = NextNodeID++;
            GameState = gs;
        }

        public GameState GameState;

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

        Dictionary<IntVec2, Station> BlockedMap;

        public CartModel(Cart cart)
        {
            this.Cart = cart;

            foreach (var it in Cart.GetTree().Root.FindChildrenByType<Station>())
            {
                foreach (var it2 in it.GetBlocked())
                {
                    BlockedMap[it2] = it;
                }
            }
        }

        public GameState Advance(GameState gs, CartAction ourAction)
        {
            var ngs = gs.Clone();
            ngs.CurrentTick++;

            foreach (var cartId in gs.CartStates.Keys.OrderBy(it => it))
            {
                if (cartId != Cart.ID)
                {
                    // this is a different cart
                    var otherCart = Cart.GetTree().Root.FindChildByPredicate<Cart>(it => it.ID == cartId);
                    var otherCartAction = otherCart.PlannedActions.ContainsKey(gs.CurrentTick) ? otherCart.PlannedActions[gs.CurrentTick] : null;
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

            return ngs;
        }

        public IEnumerable<AStarNode> GetNeighbors(AStarNode node)
        {
            if (node.GameState.CurrentTick < Cart.StartTick + CART_MAX_TICKS)
            {
                if (node.GameState.CartStates[Cart.ID].Pos.x < -5)
                {
                    // we haven't entered the map yet

                    yield return new AStarNode(Advance(node.GameState, null));
                    yield return new AStarNode(Advance(node.GameState, new CAMove { CartID = Cart.ID, Dest = new IntVec2(0, 4), Facing = 0 }));
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
                        if (i == node.GameState.CartStates[Cart.ID].Facing || node.GameState.CartStates[Cart.ID].TurnsLeft > 0)
                        {
                            yield return new AStarNode(Advance(node.GameState, new CAMove { CartID = Cart.ID, Dest = node.GameState.CartStates[Cart.ID].Pos + deltas[i], Facing = i }));
                        }
                    }

                    if (!Enumerable.SequenceEqual(node.GameState.CartStates[Cart.ID].Ings, Cart.Recipe.Ings))
                    {
                        for (var i = 0; i < 4; ++i)
                        {
                            var np = node.GameState.CartStates[Cart.ID].Pos + deltas[i];
                            if (BlockedMap.ContainsKey(np))
                            {
                                yield return new AStarNode(Advance(node.GameState, new CAUseStation { CartID = Cart.ID, StationID = BlockedMap[np].ID }));
                            }
                        }
                    }
                }
            }
        }
        public uint GetMoveCostBetweenNodes(AStarNode node1, AStarNode node2) { return 1; }
        public ulong EstimateCostBetweenNodes(AStarNode node1, AStarNode node2) { return 10_000; }
    }
}