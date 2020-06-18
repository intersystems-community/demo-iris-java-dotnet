using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Confluent.Kafka;

namespace KafkaProducer
{
    class Program
    {

        static void Produce(string topic, ClientConfig config)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                int numProduced = 0;
                int numMessages = 10;
                for (int i = 0; i < numMessages; ++i)
                {
                    var key = "alice";
                    var val = "This is an exciting message.  Number" + i + " of " + numMessages + ".  It was created " + DateTime.Now;

                    Console.WriteLine($"Producing record: {key} {val}");

                    producer.Produce(topic, new Message<string, string> { Key = key, Value = val },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.Code != ErrorCode.NoError)
                            {
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                                numProduced += 1;
                            }
                        });
                }

                producer.Flush(TimeSpan.FromSeconds(10));

                Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
            }
        }

        static void Main(string[] args)
        {
            var config = new ClientConfig
            {
                ClientId = "test-producer",
                BootstrapServers = "localhost:9092"
            };

            Produce("my-topic", config);
        }
    }
}
