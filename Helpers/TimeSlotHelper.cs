namespace SIMS.API.Helpers
{
    public record TimeSlot(int SlotNumber, TimeOnly StartTime, TimeOnly EndTime);

    public static class TimeSlotHelper
    {
        private static readonly Dictionary<int, TimeSlot> Slots = new()
        {
            { 1, new TimeSlot(1, new TimeOnly(7, 15), new TimeOnly(9, 15)) },
            { 2, new TimeSlot(2, new TimeOnly(9, 25), new TimeOnly(11, 25)) },
            { 3, new TimeSlot(3, new TimeOnly(12, 0), new TimeOnly(14, 0)) },
            { 4, new TimeSlot(4, new TimeOnly(14, 10), new TimeOnly(16, 10)) },
            { 5, new TimeSlot(5, new TimeOnly(16, 20), new TimeOnly(18, 20)) },
            { 6, new TimeSlot(6, new TimeOnly(18, 30), new TimeOnly(20, 30)) },
            { 7, new TimeSlot(7, new TimeOnly(20, 30), new TimeOnly(22, 30)) }
        };

        public static TimeSlot? GetTimeSlot(int slotNumber)
        {
            return Slots.GetValueOrDefault(slotNumber);
        }

        public static IEnumerable<TimeSlot> GetAllSlots() => Slots.Values;
    }
}