using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectManagement.Api.Controllers;
using ProjectManagement.Data.Interfaces;
using ProjectManagement.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ProjectManagement.Api.Test
{
    class BaseControllerTest
    {
        private BaseController<User> _userController;

        [SetUp]
        public void Setup()
        {
            var mockUserRepository = new Mock<IBaseRepository<User>>();
            mockUserRepository.Setup(u => u.Get()).Returns(MockUserData);
            mockUserRepository.Setup(u => u.Get(It.IsAny<long>())).Returns(MockUserData.First());
            mockUserRepository.Setup(u => u.Add(It.IsAny<User>())).Returns(MockUserData.First());
            mockUserRepository.Setup(u => u.Update(It.IsAny<User>())).Returns(MockUserData.First());
            _userController = new UserController(mockUserRepository.Object);
            _userController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Test]
        public void Should_return_list_of_entities_when_get_called()
        {
            if (_userController.Get() is OkObjectResult okObjectResult)
            {
                var result = okObjectResult.Value as List<User>;
                var expectedCount = MockUserData.Count;
                var actualCount = result.Count;
                Assert.AreEqual(expectedCount, actualCount);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void Should_return_one_entity_when_get_by_id_called()
        {
            if(_userController.Get(1) is OkObjectResult okObjectResult)
            {
                var expected = MockUserData.First();
                var actual = okObjectResult.Value as User;
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void Should_return_not_found_when_entity_not_present()
        {
            var mockRepository = new Mock<IBaseRepository<User>>();
            mockRepository.Setup(u => u.Get(It.IsAny<long>())).Returns((User)null);
            var userController = new UserController(mockRepository.Object);
            userController.ControllerContext.HttpContext = new DefaultHttpContext();
            var result = userController.Get(1234);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Should_return_no_content_when_post_called()
        {
            var result = _userController.Post(MockUserData.First());
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void Should_return_no_content_when_put_called()
        {
            var result =_userController.Put(MockUserData.First());
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void Should_return_no_content_when_delete_called()
        {
            var result = _userController.Delete(1234);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        private List<User> MockUserData
        {
            get
            {
                return new List<User>
                {
                    new User {
                        ID = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@test.com"
                    },
                    new User
                    {
                        ID = 2,
                        FirstName = "John",
                        LastName = "Skeet",
                        Email = "john.skeet@test.com"
                    },
                    new User
                    {
                        ID = 3,
                        FirstName = "Robert",
                        LastName = "Martin",
                        Email = "robert.martin@test.com"
                    },
                    new User
                    {
                        ID = 4,
                        FirstName = "Martin",
                        LastName = "Fowler",
                        Email = "martin.fowler@test.com"
                    }
                };
            }
        }
    }
}
