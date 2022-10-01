using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Cart : Spatial
{
    // carts follow a series of steps
    // carts take 1 move per 0.5 seconds

    const float CART_MOVE_TIME = 0.5f;

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

        var aStar = new AStarIndexed<AStarNode>(new CartModel(this));
    }

    struct AStarNode : IEquatable<AStarNode>, IComparable<AStarNode>
    {
        public GameState GameState;

        public bool Equals(AStarNode other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(AStarNode other)
        {
            throw new NotImplementedException();
        }
    }

    class CartModel : AStarIndexed<AStarNode>.IModel
    {
        Cart Cart;

        public CartModel(Cart cart)
        {
            this.Cart = cart;
        }

        public GameState Advance(GameState gs, CartAction ourAction)
        {
            var ngs = gs.Clone();

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
        }

        public IEnumerable<AStarNode> GetNeighbors(AStarNode node)
        {
            if (node.GameState.CartStates[Cart.ID].Pos.x < -5)
            {
                // we haven't entered the map yet

            }

        }
        public uint GetMoveCostBetweenNodes(AStarNode node1, AStarNode node2) { throw new NotImplementedException(); }
        public ulong EstimateCostBetweenNodes(AStarNode node1, AStarNode node2) { throw new NotImplementedException(); }
    }
}