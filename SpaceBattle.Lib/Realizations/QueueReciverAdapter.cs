namespace SpaceBattle.Lib;

public class QueueRecieverAdapter : IReciever
{
    Queue<ICommand> queue;

    public QueueRecieverAdapter(Queue<ICommand> queue) {
        this.queue = queue;
    };

    public ICommand Recieve()
    {
        return queue.Dequeue();
    }

    public bool isEmpty()
    {
        return queue.Count() == 0;
    }
}