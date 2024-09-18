App.New(new NodeBuilder().AddAction(r => Console.WriteLine(r.Url)));
App.Current.Recive(new Request {
    Url = "localhost/endpoint"
});

public class Request
{
    public required string Url { get; set; }
}

public abstract class Node
{
    public abstract void Recive(Request request);
}

public class ActionNode(Action<Request> action) : Node
{
    public override void Recive(Request request)
        => action(request);
}

public class FilterNode(Func<Request, bool> condition, Node child) : Node
{
    public readonly Node Children = child;
    public override void Recive(Request request)
    {
        if (condition(request))
            Children.Recive(request);
    }
}

public class ParentNode(IEnumerable<Node> nodes) : Node
{
    public readonly Node[] Children = [ ..nodes ];
    public override void Recive(Request request)
    {
        foreach (var child in Children)
            child.Recive(request);
    }
}

public class NodeBuilder
{
    Node? main;

    public NodeBuilder AddAction(Action<Request> action)
    {
        main = new ActionNode(action);
        return this;
    }

    public Node? GetNode()
        => main;
}

public class App(Node main)
{
    private static readonly App? current = null;
    public static App Current => current ?? throw new Exception();

    public static void New(NodeBuilder nodeBuilder)
        => nodeBuilder.GetNode();
    
    public void Recive(Request request)
        => main.Recive(request);
}