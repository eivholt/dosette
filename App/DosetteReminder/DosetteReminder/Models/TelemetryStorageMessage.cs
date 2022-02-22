using System.Text.Json.Serialization;

namespace DosetteReminder.Models { 
    public class ApplicationIds
    {
        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; }
    }

    public class EndDeviceIds
    {
        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        [JsonPropertyName("application_ids")]
        public ApplicationIds ApplicationIds { get; set; }

        [JsonPropertyName("dev_eui")]
        public string DevEui { get; set; }

        [JsonPropertyName("dev_addr")]
        public string DevAddr { get; set; }
    }

    public class Lids
    {
        [JsonPropertyName("lid1")]
        public bool Lid1 { get; set; }

        [JsonPropertyName("lid2")]
        public bool Lid2 { get; set; }

        [JsonPropertyName("lid3")]
        public bool Lid3 { get; set; }

        [JsonPropertyName("lid4")]
        public bool Lid4 { get; set; }
    }

    public class DecodedPayload
    {
        [JsonPropertyName("lids")]
        public Lids Lids { get; set; }
    }

    public class GatewayIds
    {
        [JsonPropertyName("gateway_id")]
        public string GatewayId { get; set; }

        [JsonPropertyName("eui")]
        public string Eui { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public int Altitude { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    public class PacketBroker
    {
        [JsonPropertyName("message_id")]
        public string MessageId { get; set; }

        [JsonPropertyName("forwarder_net_id")]
        public string ForwarderNetId { get; set; }

        [JsonPropertyName("forwarder_tenant_id")]
        public string ForwarderTenantId { get; set; }

        [JsonPropertyName("forwarder_cluster_id")]
        public string ForwarderClusterId { get; set; }

        [JsonPropertyName("forwarder_gateway_eui")]
        public string ForwarderGatewayEui { get; set; }

        [JsonPropertyName("forwarder_gateway_id")]
        public string ForwarderGatewayId { get; set; }

        [JsonPropertyName("home_network_net_id")]
        public string HomeNetworkNetId { get; set; }

        [JsonPropertyName("home_network_tenant_id")]
        public string HomeNetworkTenantId { get; set; }

        [JsonPropertyName("home_network_cluster_id")]
        public string HomeNetworkClusterId { get; set; }
    }

    public class RxMetadata
    {
        [JsonPropertyName("gateway_ids")]
        public GatewayIds GatewayIds { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("rssi")]
        public int Rssi { get; set; }

        [JsonPropertyName("channel_rssi")]
        public int ChannelRssi { get; set; }

        [JsonPropertyName("snr")]
        public double Snr { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("channel_index")]
        public int ChannelIndex { get; set; }

        [JsonPropertyName("packet_broker")]
        public PacketBroker PacketBroker { get; set; }
    }

    public class Lora
    {
        [JsonPropertyName("bandwidth")]
        public int Bandwidth { get; set; }

        [JsonPropertyName("spreading_factor")]
        public int SpreadingFactor { get; set; }
    }

    public class DataRate
    {
        [JsonPropertyName("lora")]
        public Lora Lora { get; set; }
    }

    public class Settings
    {
        [JsonPropertyName("data_rate")]
        public DataRate DataRate { get; set; }

        [JsonPropertyName("coding_rate")]
        public string CodingRate { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }
    }

    public class User
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public int Altitude { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    public class Locations
    {
        [JsonPropertyName("user")]
        public User User { get; set; }
    }

    public class NetworkIds
    {
        [JsonPropertyName("net_id")]
        public string NetId { get; set; }

        [JsonPropertyName("tenant_id")]
        public string TenantId { get; set; }

        [JsonPropertyName("cluster_id")]
        public string ClusterId { get; set; }
    }

    public class UplinkMessage
    {
        [JsonPropertyName("f_port")]
        public int FPort { get; set; }

        [JsonPropertyName("f_cnt")]
        public int FCnt { get; set; }

        [JsonPropertyName("frm_payload")]
        public string FrmPayload { get; set; }

        [JsonPropertyName("decoded_payload")]
        public DecodedPayload DecodedPayload { get; set; }

        [JsonPropertyName("rx_metadata")]
        public List<RxMetadata> RxMetadata { get; set; }

        [JsonPropertyName("settings")]
        public Settings Settings { get; set; }

        [JsonPropertyName("received_at")]
        public string ReceivedAt { get; set; }

        [JsonPropertyName("consumed_airtime")]
        public string ConsumedAirtime { get; set; }

        [JsonPropertyName("locations")]
        public Locations Locations { get; set; }

        [JsonPropertyName("network_ids")]
        public NetworkIds NetworkIds { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("end_device_ids")]
        public EndDeviceIds EndDeviceIds { get; set; }

        [JsonPropertyName("received_at")]
        public string ReceivedAt { get; set; }

        [JsonPropertyName("uplink_message")]
        public UplinkMessage UplinkMessage { get; set; }
    }

    public class TelemetryStorageMessage
    {
        [JsonPropertyName("result")]
        public Result Result { get; set; }
    }

}