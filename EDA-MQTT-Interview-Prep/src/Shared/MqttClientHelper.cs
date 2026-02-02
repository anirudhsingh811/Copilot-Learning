using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;

namespace Shared;

public class MqttClientHelper
{
    private readonly IMqttClient _client;
    private readonly string _brokerHost;
    private readonly int _brokerPort;

    public MqttClientHelper(string brokerHost = "localhost", int brokerPort = 1883)
    {
        _brokerHost = brokerHost;
        _brokerPort = brokerPort;
        var factory = new MqttFactory();// Create MQTT client and configure options
        _client = factory.CreateMqttClient();// Create MQTT client
    }    public async Task ConnectAsync()
    {
        var options = new MqttClientOptionsBuilder().WithTcpServer(_brokerHost, _brokerPort).Build();//Optins for connecting to the MQTT broker such as host and port
        await _client.ConnectAsync(options, CancellationToken.None);// Connect to the MQTT broker and cancellation token for async operation
    }
    public async Task PublishAsync<T>(string topic, T message)
    {
        var payload = JsonSerializer.Serialize(message);// Serialize message to JSON
        var mqttMessage = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(payload).Build();// Build MQTT message with topic and payload
        await _client.PublishAsync(mqttMessage);// Publish the message to the specified topic
    }
    public async Task SubscribeAsync(string topic, Func<string, Task> handler)
    {
        _client.ApplicationMessageReceivedAsync += async e => { if (e.ApplicationMessage.Topic == topic) { var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload); await handler(payload); } };// Subscribe to topic
        await _client.SubscribeAsync(topic);
    }
    public async Task DisconnectAsync() => await _client.DisconnectAsync();// Disconnect from the MQTT broker
}