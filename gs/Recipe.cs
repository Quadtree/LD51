public class Recipe
{
    public enum Ing
    {
        None,
        Lettuce,
    }

    public enum Op
    {
        None,
        Cook,
        Chop
    }

    public struct Step
    {
        Ing Ing;
        Op Op;
    }

    public Step[] Steps;
}