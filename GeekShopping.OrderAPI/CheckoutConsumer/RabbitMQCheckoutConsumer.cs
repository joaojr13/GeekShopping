using GeekShopping.CartAPI.Repository;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Models;
using GeekShopping.OrderAPI.RabbitMQSender;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.CheckoutConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {
        private readonly OrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;
        private IRabbitMQMessageSender _rabbitMQMessageSender;
        public RabbitMQCheckoutConsumer(OrderRepository orderRepository, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _rabbitMQMessageSender = rabbitMQMessageSender;
            _orderRepository = orderRepository;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "checkoutqueue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (channel, evt) => 
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());

                CheckoutHeaderVO vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);
                ProcessOrder(vo).GetAwaiter().GetResult();

                _channel.BasicAck(evt.DeliveryTag, false);
            };

            _channel.BasicConsume("checkoutqueue", autoAck: false, consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessOrder(CheckoutHeaderVO vo)
        {
            OrderHeader order = new()
            {
                UserId = vo.UserId,
                FirstName = vo.FirstName,
                LastName = vo.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = vo.CardNumber,
                CouponCode = vo.CouponCode,
                CVV = vo.CVV,
                DiscountTotal = vo.DiscountTotal,
                Email = vo.Email,
                ExpiryMonthYear =   vo.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                PurchaseAmount = vo.PurchaseAmount,
                PaymentStatus = false,
                Phone = vo.Phone,
                DateTime = vo.DateTime,
            };

            foreach (var details in vo.CartDetails)
            {
                OrderDetails detail = new()
                {
                    ProductId = details.ProductId,
                    ProductName = details.Product.Name,
                    Price = details.Product.Price,
                    Count = details.Count,
                };

                order.CartTotalItens += details.Count; 

                order.OrderDetails.Add(detail);
            }

            await _orderRepository.AddOrder(order);

            PaymentVO payment = new()
            {
                Name = order.FirstName + "" + order.LastName,
                CardNumber = order.CardNumber,
                CVV = order.CVV,
                ExpiryMonthYear= order.ExpiryMonthYear,
                OrderId = order.Id,
                PurchaseAmount= order.PurchaseAmount,
                Email = order.Email,
            };

            try
            {
                _rabbitMQMessageSender.SendMessage(payment, "orderpaymentprocessqueue");
            }
            catch (Exception)
            {
                //log
                throw;
            }
        }
    }
}
