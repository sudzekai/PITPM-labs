using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Interfaces
{
    /// <summary>
    /// Интерфейс шины сообщений, используемой для уведомлений других сервисов.
    /// </summary>
    public interface IMessageBus
    {
        Task PublishAsync(string topic, object payload);
    }
}
