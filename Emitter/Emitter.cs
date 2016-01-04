namespace Emitter.Network
{
    /// <summary>
    /// Represents emitter.io MQTT-based client.
    /// </summary>
    public class Emitter : MqttClient
    {
        public Emitter() : base("api.emitter.io")
        {

        }
    }
}