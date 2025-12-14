using FluentAssertions;
using Moq;
using OrderProcessing.Interfaces;
using OrderProcessing.Models;
using OrderProcessing.Services;
using System.Runtime.InteropServices;

namespace LabWork7
{
    public class OrderProcessingTester
    {
        private readonly Mock<ICustomerRepository> _customersMock = new();
        private readonly Mock<IOrderRepository> _ordersMock = new();
        private readonly Mock<IMessageBus> _busMock = new();

        private readonly OrderService _orderService;

        public OrderProcessingTester()
        {
            _orderService = new
                (
                    _ordersMock.Object,
                    _customersMock.Object,
                    _busMock.Object
                );
        }

        //CreateOrder если юзер есть
        [Fact]
        public async Task CreateOrder_Good()
        {
            var customerId = Guid.NewGuid();

            _customersMock.Setup(c => c.GetByIdAsync(customerId))
                          .ReturnsAsync(new Customer { Id = customerId });

            var order = await _orderService.CreateOrderAsync(customerId, 3000);
            
            order.CustomerId.Should().Be(customerId);
            order.TotalAmount.Should().Be(3000);

            _ordersMock.Verify(o => o.AddAsync(It.IsAny<Order>()));
            _busMock.Verify(b => b.PublishAsync("order.created", It.IsAny<object>()));
        }

        // CreateOrder юзера нет
        [Fact]
        public async Task CreateOrder_Exception()
        {
            _customersMock.Setup(c => c.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((Customer)null);

            Func<Task> act = async () =>
                await _orderService.CreateOrderAsync(Guid.NewGuid(), 3000);

            await act.Should().ThrowAsync<InvalidOperationException>()
                              .WithMessage("Клиент не найден.");
        }

        // ConfrimPayment заказ существует
        [Fact]
        public async Task ConfirmPayment_Good()
        {
            var orderId = Guid.NewGuid();

            var order = new Order() { Id = orderId, Status = OrderStatus.Pending };

            _ordersMock.Setup(o => o.GetByIdAsync(orderId))
                       .ReturnsAsync(order);

            await _orderService.ConfirmPaymentAsync(orderId);

            order.Status.Should().Be(OrderStatus.Paid);
            _ordersMock.Verify(o => o.UpdateAsync(order));
            _busMock.Verify(b => b.PublishAsync("order.paid", It.IsAny<object>()));
        }

        // ConfrimPayment заказа нет
        [Fact]
        public async Task ConfirmPayment_Exception()
        {
            _ordersMock.Setup(o => o.GetByIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Order)null);

            Func<Task> act = async () =>
                await _orderService.ConfirmPaymentAsync(It.IsAny<Guid>());

            await act.Should().ThrowAsync<InvalidOperationException>()
                              .WithMessage("Заказ не найден.");
        }

        //CancelOrder заказ есть
        [Fact]
        public async Task CancelOrder_Good()
        {
            var orderId = Guid.NewGuid();

            var order = new Order() { Id = orderId, Status = OrderStatus.Paid };

            _ordersMock.Setup(o => o.GetByIdAsync(orderId))
                       .ReturnsAsync(order);

            await _orderService.CancelOrderAsync(orderId);

            order.Status.Should().Be(OrderStatus.Cancelled);
            _ordersMock.Verify(o => o.UpdateAsync(order));
            _busMock.Verify(b => b.PublishAsync("order.cancelled", It.IsAny<object>()));
        }

        //CancelOrder заказа нет
        [Fact]
        public async Task CancelOrder_Exception()
        {
            _ordersMock.Setup(o => o.GetByIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Order)null);

            Func<Task> act = async () =>
                await _orderService.CancelOrderAsync(It.IsAny<Guid>());

            await act.Should().ThrowAsync<InvalidOperationException>()
                              .WithMessage("Заказ не найден.");
        }

        // ShipOrder заказ есть
        [Fact]
        public async Task ShipOrder_Good()
        {
            var orderId = Guid.NewGuid();

            var order = new Order() { Id = orderId, Status = OrderStatus.Paid };

            _ordersMock.Setup(o => o.GetByIdAsync(orderId))
                       .ReturnsAsync(order);

            await _orderService.ShipOrderAsync(orderId);

            order.Status.Should().Be(OrderStatus.Shipped);
            _ordersMock.Verify(o => o.UpdateAsync(order));
            _busMock.Verify(b => b.PublishAsync("order.shipped", It.IsAny<object>()));
        }

        // ShipOrder заказ есть но не оплачен
        [Fact]
        public async Task ShipOrder_NotPaid_Exception()
        {
            var orderId = Guid.NewGuid();

            var order = new Order() { Id = orderId, Status = OrderStatus.Pending };

            _ordersMock.Setup(o => o.GetByIdAsync(orderId))
                       .ReturnsAsync(order);
            Func<Task> act = async () =>
                await _orderService.ShipOrderAsync(orderId);

            await act.Should().ThrowAsync<InvalidOperationException>()
                              .WithMessage("Отправить можно только оплаченные заказы.");
        }

        // ShipOrder заказа нет
        [Fact]
        public async Task ShipOrder_Exception()
        {
            _ordersMock.Setup(o => o.GetByIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync((Order)null);

            Func<Task> act = async () => 
                await _orderService.ShipOrderAsync(It.IsAny<Guid>());

            await act.Should().ThrowAsync<InvalidOperationException>()
                              .WithMessage("Заказ не найден.");
        }
    }
}
