namespace BookService.Services.MQ;

using Models;

public interface IMessageBusService {
    Task PublishBookCreated(Book book);
    Task PublishBookUpdated(Book book);
    Task PublishBookDeleted(int id);
}
