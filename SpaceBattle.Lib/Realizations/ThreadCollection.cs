namespace SpaceBattle.Lib;


public class ThreadCollection
{
    public MyThread? tread;
    public IReciever? innerReciever;
    public ISender? innerSender;
    public IReciever? outerReciever;
    public ISender? outerSender;

    public ThreadCollection() { }
}

