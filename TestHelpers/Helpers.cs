using AutoMapper;
using ElektaAppointmentSystemAPI.MappingProfiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElektaAppointmentSystemAPI.Tests.TestHelpers
{
    public class Helpers
    {
        public static IMapper GetAppointmentMapperForTest()
        {
            var apptProfile = new AppointmentProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(apptProfile));
            return new Mapper(configuration);
        }

        public static IMapper GetChangedAppointmentMapperForTest()
        {
            var apptProfile = new ChangedAppointmentProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(apptProfile));
            return new Mapper(configuration);
        }
    }
}
