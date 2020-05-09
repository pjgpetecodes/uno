﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using CoreMidi;
using Uno.Devices.Enumeration.Internal;
using Uno.Devices.Enumeration.Internal.Providers.Midi;
using Windows.Storage.Streams;

namespace Windows.Devices.Midi
{
    public partial class MidiInPort
    {
		private MidiEndpoint _endpoint;
		private MidiClient _client;
		private MidiPort _port;

		private MidiInPort(string deviceId, MidiEndpoint endpoint)
		{
			DeviceId = deviceId;
			_endpoint = endpoint;
			_client = new MidiClient(Guid.NewGuid().ToString());
		}

		internal void Open()
		{
			_port = _client.CreateInputPort(_endpoint.EndpointName);
			_port.MessageReceived += NativePortMessageReceived;
		}

		private void NativePortMessageReceived(object sender, MidiPacketsEventArgs e)
		{
			foreach (var packet in e.Packets)
			{
				var bytes = new byte[packet.Length];
				Marshal.Copy(packet.Bytes, bytes, 0, packet.Length);
				OnMessageReceived(bytes, TimeSpan.FromMilliseconds(packet.TimeStamp));
			}
		}

		public void Dispose()
		{
			_port?.Disconnect(_endpoint);
			_port?.Dispose();
			_client?.Dispose();
			_endpoint?.Dispose();
			_port = null;
			_client = null;
			_endpoint = null;
		}

		private static async Task<MidiInPort> FromIdInternalAsync(DeviceIdentifier identifier)
		{
			var provider = new MidiInDeviceClassProvider();
			var nativeDeviceInfo = provider.GetNativeEndpoint(identifier.Id);
			if (nativeDeviceInfo == null)
			{
				throw new InvalidOperationException(
					"Given MIDI out device does not exist or is no longer connected");
			}

			var port = new MidiInPort(identifier.ToString(), nativeDeviceInfo);
			port.Open();
			return port;
		}
	}
}
