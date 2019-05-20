using System;
using AkkaTest.Actors;
using AkkaTest.Messages;
using Xunit;
using Akka.TestKit.Xunit2;
using FluentAssertions;

namespace AkkaTest.Tests
{
    public class UnitTests: TestKit
    {
        [Fact]
        public void Device_actor_must_reply_with_empty_reading_if_no_temperature_is_known()
        {
            var probe = CreateTestProbe();
            var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new ReadTemperature(requestId: 42), probe.Ref);
            var response = probe.ExpectMsg<RespondTemperature>();
            response.RequestId.Should().Be(42);
            response.Value.Should().BeNull();
        }

        [Fact]
        public void Device_actor_must_reply_with_latest_temperature_reading()
        {
            var probe = CreateTestProbe();
            var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new RecordTemperature(requestId: 1, value: 24.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);

            deviceActor.Tell(new ReadTemperature(requestId: 2), probe.Ref);
            var response1 = probe.ExpectMsg<RespondTemperature>();
            response1.RequestId.Should().Be(2);
            response1.Value.Should().Be(24.0);

            deviceActor.Tell(new RecordTemperature(requestId: 3, value: 55.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 3);

            deviceActor.Tell(new ReadTemperature(requestId: 4), probe.Ref);
            var response2 = probe.ExpectMsg<RespondTemperature>();
            response2.RequestId.Should().Be(4);
            response2.Value.Should().Be(55.0);
        }

        [Fact]
        public void Device_actor_must_reply_to_registration_requests()
        {
            var probe = CreateTestProbe();
            var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new RequestTrackDevice("group", "device"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            probe.LastSender.Should().Be(deviceActor);
        }

        [Fact]
        public void Device_actor_must_ignore_wrong_registration_requests()
        {
            var probe = CreateTestProbe();
            var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new RequestTrackDevice("wrongGroup", "device"), probe.Ref);
            probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));

            deviceActor.Tell(new RequestTrackDevice("group", "Wrongdevice"), probe.Ref);
            probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
        }

        [Fact]
        public void DeviceGroup_actor_must_be_able_to_register_a_device_actor()
        {
            var probe = CreateTestProbe();
            var groupActor = Sys.ActorOf(DeviceGroup.Props("group"));

            groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor1 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor2 = probe.LastSender;
            deviceActor1.Should().NotBe(deviceActor2);

            // Check that the device actors are working
            deviceActor1.Tell(new RecordTemperature(requestId: 0, value: 1.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 0);
            deviceActor2.Tell(new RecordTemperature(requestId: 1, value: 2.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);
        }

        [Fact]
        public void DeviceGroup_actor_must_ignore_requests_for_wrong_groupId()
        {
            var probe = CreateTestProbe();
            var groupActor = Sys.ActorOf(DeviceGroup.Props("group"));

            groupActor.Tell(new RequestTrackDevice("wrongGroup", "device1"), probe.Ref);
            probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
        }
    }
}