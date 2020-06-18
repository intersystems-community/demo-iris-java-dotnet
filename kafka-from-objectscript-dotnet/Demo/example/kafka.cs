using System;
using System.Threading;
using System.Diagnostics;
using System.IO;

using Confluent.Kafka;
using InterSystems.Data.IRISClient.ADO;
using InterSystems.Data.IRISClient.Gateway;

namespace example
{
    public class kafka
    {
        private ConsumerConfig conf = new ConsumerConfig
        {
            GroupId = "test-consumer-group",
            BootstrapServers = "localhost:9092"
        };

        private IConsumer<Ignore, string> consumer = null;

        public void startConsumer(string topicName)
        {
            consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
            consumer.Subscribe(topicName);
        }

        public void consumePendingMessages()
        {
            bool done = false;

            while (!done)
            {
                var cr = consumer.Consume(10);
                done = (cr == null);
                if (!done)
                {
                    processMessage(cr);
                }
            }
        }

        // Write a message that comes from Kafka to a SQL table in IRIS using ObjectScript
        public void processMessage(ConsumeResult<Ignore, string> cr)
        {
            var iris = GatewayContext.GetIRIS();
            var test = (IRISObject)iris.ClassMethodObject("Example.Messages", "%New");

            test.Set("Body", cr.Message.Value);
            test.InvokeVoid("%Save");
        }


        public void stopConsumer()
        {
            consumer.Close();
        }

    }
}
