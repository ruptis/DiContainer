using DiContainer.Resolving;
var builder = new ContainerBuilder();

builder.Register<A>(Lifetime.Transient);
builder.Register<B>(Lifetime.Transient);
builder.Register<C>(Lifetime.Transient);
builder.Register<D>(Lifetime.Transient);
builder.Register(typeof(Generic<>), Lifetime.Transient);

Container container = builder.Build();

var a = container.Resolve<A>();
var generic = container.Resolve<Generic<A>>();

Console.WriteLine(a);
Console.WriteLine(generic);

public class Generic<T> { }

public class A
{
    public A(B b) { }
}

public class B
{
    public B(C c) { }
}

public class C
{
    public C(D d) { }
}

public class D;
