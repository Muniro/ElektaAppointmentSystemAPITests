using ElektaAppointmentSystemAPI.DataHelpers;
using ElektaAppointmentSystemAPI.Entity;
using ElektaAppointmentSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElektaAppointmentSystemAPI.Tests.Fakes
{
    public static class FakeBookings
    {
        /* Helper methods for generating test data */

        public static string GetFakeBookingsAsJson()
        {
            return @"[
                   {         'PatientID': 1,
                    'AppointmentDateTime': '2020-08-11T08:00:00Z'
                  },
                  {
                                'PatientID': 2,
                    'AppointmentDateTime': '2020-08-10T08:00:00Z'
                  },
                  {
                                'PatientID': 3,
                    'AppointmentDateTime': '2020-08-10T08:00:00Z'
                  },
                  {
                                'PatientID': 4,
                    'AppointmentDateTime': '2020-08-11T12:00:00Z'
            ]";
        }

        public static AppointmentDto GetFakeAppointmentDto()
        {
            var rand = new Random(100);
            return new AppointmentDto() { PatientID = rand.Next(100,300), AppointmentDateTime = DateTime.Now.AddDays(rand.Next(1,13))};

        }

        public static List<AppointmentEntity> GetFakeBookingsAsList()
        {

            var fakeAppt1 = new AppointmentEntity() { PatientID = 1, AppointmentDateTime = DateTime.Now };
            var fakeAppt2 = new AppointmentEntity() { PatientID = 2, AppointmentDateTime = DateTime.Now.AddDays(1) };
            var fakeAppt3 = new AppointmentEntity() { PatientID = 3, AppointmentDateTime = DateTime.Now .AddDays(3)};

            var list = new List<AppointmentEntity>();
            list.Add(fakeAppt1);
            list.Add(fakeAppt2);
            list.Add(fakeAppt3);

            return list;
        }
    }
}