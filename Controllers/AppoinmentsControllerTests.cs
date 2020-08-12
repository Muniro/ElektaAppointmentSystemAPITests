using AutoMapper;
using ElektaAppointmentSystemAPI.Entity;
using ElektaAppointmentSystemAPI.Helpers;
using ElektaAppointmentSystemAPI.Models;
using ElektaAppointmentSystemAPI.Repositories;
using ElektaAppointmentSystemAPI.Tests.Fakes;
using ElektaAppointmentSystemAPI.v1.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace ElektaAppointmentSystemAPI.Tests.Controllers
{
    public class AppoinmentsControllerTests
    {

        private readonly IAppointmentRepository _repo;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly INotifier _notify;
        private readonly IMapper _mapper;

        private readonly AppointmentsController controller;

        public AppoinmentsControllerTests()
        {
            _repo = NSubstitute.Substitute.For<IAppointmentRepository>();
            _logger = NSubstitute.Substitute.For<ILogger<AppointmentsController>>();
            _notify = NSubstitute.Substitute.For<INotifier>();
            _mapper = NSubstitute.Substitute.For<IMapper>();
            controller = new AppointmentsController(_repo, _notify, _mapper, _logger);
        }

        [Fact]
        public  void GetTodaysAppointments_WhenAppointmentForTodayIsFound_ReturnsOK()
        {
            // Arrange
            var bookings = FakeBookings.GetFakeBookingsAsList();
            _repo.GetAppointmentsAsync().ReturnsForAnyArgs(bookings);

            var expectedCodeResult = new StatusCodeResult(200);

            // Act
            var sut = controller.GetTodaysAppointments();

            var okResult = sut as OkObjectResult;


            // Assert
            Assert.NotNull(okResult);
            Assert.Equal(expectedCodeResult.StatusCode, okResult.StatusCode);
            
        }

        [Fact]
        public  void GetTodaysAppointments_WhenAppointmentForTodayIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var bookings = FakeBookings.GetFakeBookingsAsList();

            //we will remove any today's bookings to test this scenario
            var notTodaysBookings = bookings.Where(x => x.AppointmentDateTime.Date != DateTime.Today);
             _repo.GetAppointmentsAsync().Returns(notTodaysBookings);

           
            var expectedCodeResult = new StatusCodeResult(404);

            // Act
            var sut = controller.GetTodaysAppointments();

            var notFoundResult = sut as NotFoundObjectResult;


            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(expectedCodeResult.StatusCode, notFoundResult.StatusCode);

        }

        [Fact]
        public async void CreateAppointment_WhenCalledWithValidAppointment_ReturnsCreated()
        {
            // Arrange   
            var apptItem = new AppointmentEntity()
            {
                PatientID = 200,
                AppointmentDateTime = DateTime.Now.AddDays(1)
              
            
            };
            var apptDtoItem = FakeBookings.GetFakeAppointmentDto();
            var mapper = TestHelpers.Helpers.GetAppointmentMapperForTest();
            _repo.CreateBookingAsync(apptItem).ReturnsForAnyArgs(true);

            var mycontroller = new AppointmentsController(_repo, _notify, mapper, _logger);
            var expectedCodeResult = new StatusCodeResult(201);

     
            // Act
            var sut = await mycontroller.Create(apptDtoItem);

            var result = sut as StatusCodeResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCodeResult.StatusCode, result.StatusCode);
        }


        [Fact]
        public async void CreateAppointment_WhenCalledWithInvalidAppointment_ReturnsBadRequest()
        {
            // Arrange   
            var apptItem = new AppointmentEntity()
            {
                PatientID = 200,
                AppointmentDateTime = DateTime.Now.AddDays(20) /* Invalid data: Rule is appointment can be booked upto two weeks ahead only */
               

            };
            var apptDtoItem = FakeBookings.GetFakeAppointmentDto();
            var mapper = TestHelpers.Helpers.GetAppointmentMapperForTest();
            _repo.CreateBookingAsync(apptItem).ReturnsForAnyArgs(false);

            var mycontroller = new AppointmentsController(_repo, _notify, mapper, _logger);
            var expectedCodeResult = new StatusCodeResult(400);


            // Act
            var sut = await mycontroller.Create(apptDtoItem);

            var result = sut as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCodeResult.StatusCode, result.StatusCode);
        }



        [Fact]
        public async void CancelAppointment_WhenCalledWithValidAppointment_ReturnsOk()
        {
            // Arrange   
            var apptItem = new AppointmentEntity()
            {
                PatientID = 200,
                AppointmentDateTime = DateTime.Now.AddDays(1)
               

            };
            var apptDtoItem = FakeBookings.GetFakeAppointmentDto();
            var mapper = TestHelpers.Helpers.GetAppointmentMapperForTest();
            _repo.CancelBookingAsync(apptItem).ReturnsForAnyArgs(true);

            var mycontroller = new AppointmentsController(_repo, _notify, mapper, _logger);
            var expectedCodeResult = new StatusCodeResult(200);

            // Act
            var sut = await mycontroller.CancelAppointment(apptDtoItem);

            var result = sut as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCodeResult.StatusCode, result.StatusCode);

        }

        [Fact]
        public async void CancelAppointment_WhenCalledWithInvalidAppointment_ReturnsBadRequest()
        {
            // Arrange   
            var apptItem = new AppointmentEntity()
            {
                PatientID = 200,
                AppointmentDateTime = DateTime.Now.AddDays(20) //invalid appointment max today + 14 days


            };
            var apptDtoItem = FakeBookings.GetFakeAppointmentDto();
            var mapper = TestHelpers.Helpers.GetAppointmentMapperForTest();
            _repo.CancelBookingAsync(apptItem).ReturnsForAnyArgs(false);

            var mycontroller = new AppointmentsController(_repo, _notify, mapper, _logger);
            var expectedCodeResult = new StatusCodeResult(400);

            // Act
            var sut = await mycontroller.CancelAppointment(apptDtoItem);

            var result = sut as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCodeResult.StatusCode, result.StatusCode);

        }


        [Fact]
        public async void UpdateAppointment_WhenCalledWithValidAppointment_ReturnsOk()
        {
            // Arrange   
            var apptItem = new ChangedAppointmentDto()
            {
                PatientID = 12,
                AppointmentDateTime = new DateTime(2020,08,12,13,30,00),
                NewAppointmentDateTime = new DateTime(2020, 08, 12, 14, 30, 00)


            };
            var apptItemEntity= new ChangedAppointmentEntity()
            {
                PatientID = 12,
                AppointmentDateTime = new DateTime(2020, 08, 12, 13, 30, 00),
                NewAppointmentDateTime= new DateTime(2020, 08, 12, 14, 30, 00)


            };

            var mapper = TestHelpers.Helpers.GetChangedAppointmentMapperForTest();

            _repo.UpdateBookingAsync(apptItemEntity).ReturnsForAnyArgs(true);

            var mycontroller = new AppointmentsController(_repo, _notify, mapper, _logger);
            var expectedCodeResult = new StatusCodeResult(200);

            // Act
            var sut = await mycontroller.UpdateAppointment(apptItem);

            var result = sut as OkResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCodeResult.StatusCode, result.StatusCode);

        }


        [Fact]
        public async void UpdateAppointment_WhenCalledWithValidAppointment_ReturnsBadRequest()
        {
            // Arrange   
            var apptItem = new ChangedAppointmentDto()
            {
                PatientID = 12,
                AppointmentDateTime = new DateTime(2020, 08, 12, 13, 30, 00),
                NewAppointmentDateTime = new DateTime(2020, 08, 12, 14, 30, 00)


            };
            var apptItemEntity = new ChangedAppointmentEntity()
            {
                PatientID = 12,
                AppointmentDateTime = new DateTime(2020, 08, 12, 13, 30, 00),
                NewAppointmentDateTime = new DateTime(2020, 08, 12, 14, 30, 00)


            };

            var mapper = TestHelpers.Helpers.GetChangedAppointmentMapperForTest();

            _repo.UpdateBookingAsync(apptItemEntity).ReturnsForAnyArgs(false);

            var mycontroller = new AppointmentsController(_repo, _notify, mapper, _logger);
            var expectedCodeResult = new StatusCodeResult(400);

            // Act
            var sut = await mycontroller.UpdateAppointment(apptItem);

            var result = sut as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCodeResult.StatusCode, result.StatusCode);

        }





    }
}
