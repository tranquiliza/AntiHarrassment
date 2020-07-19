using AntiHarassment.Core.Models;
using AntiHarassment.Core.Security;
using AntiHarassment.Messaging.NServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiHarassment.Core.Tests
{
    [TestClass]
    public class SuspensionServiceTests
    {
        [TestMethod]
        public async Task AdminShouldAuthorize()
        {
            // arrange
            var suspensionRepositoryMock = new Mock<ISuspensionRepository>();
            suspensionRepositoryMock.Setup(x => x.GetSuspensionsForChannel(It.IsAny<string>())).ReturnsAsync(new List<Suspension>());
            var channelRepository = new Mock<IChannelRepository>();
            channelRepository.Setup(x => x.GetChannel(It.IsAny<string>())).ReturnsAsync(new Channel("Tranquiliza", true));
            var context = new Mock<IApplicationContext>();
            context.Setup(x => x.User).Returns(CreateAdminUserFromJson());

            var tagRepository = new Mock<ITagRepository>();
            var messageDispatcher = new Mock<IMessageDispatcher>();


            var service = new SuspensionService(suspensionRepositoryMock.Object, channelRepository.Object, tagRepository.Object, messageDispatcher.Object);

            // act
            var result = await service.GetAllSuspensionsAsync("Tranquiliza", context.Object).ConfigureAwait(false);

            // assert
            Assert.AreEqual(result.State, ResultState.NoContent);
        }

        private User CreateAdminUserFromJson()
        {
            const string json = @"{""Id"":""47afc916-439c-4f57-82b3-0d47c9103390"",""TwitchUsername"":""sven"",""Email"":null,""PasswordHash"":"""",""PasswordSalt"":"""",""roles"":[""ADMIN""],""EmailConfirmed"":true,""EmailConfirmationToken"":""721694ff-1032-462b-b9b9-aece212da208"",""ResetToken"":""484d94a6-ae84-476a-85fc-7a4afc55c030"",""ResetTokenExpiration"":""2020-07-13T01:06:38.3689257Z""}";

            return Serialization.Deserialize<User>(json);
        }

        [TestMethod]
        public async Task OwnerShouldAuthorize()
        {
            // arrange
            var suspensionRepositoryMock = new Mock<ISuspensionRepository>();
            suspensionRepositoryMock.Setup(x => x.GetSuspensionsForChannel(It.IsAny<string>())).ReturnsAsync(new List<Suspension>());
            var channelRepository = new Mock<IChannelRepository>();
            channelRepository.Setup(x => x.GetChannel(It.IsAny<string>())).ReturnsAsync(new Channel("Tranquiliza", true));
            var context = new Mock<IApplicationContext>();
            context.Setup(x => x.User).Returns(CreateOwnerUserFromJson());
            var tagRepository = new Mock<ITagRepository>();
            var messageDispatcher = new Mock<IMessageDispatcher>();

            var service = new SuspensionService(suspensionRepositoryMock.Object, channelRepository.Object, tagRepository.Object, messageDispatcher.Object);

            // act
            var result = await service.GetAllSuspensionsAsync("Tranquiliza", context.Object).ConfigureAwait(false);

            // assert
            Assert.AreEqual(result.State, ResultState.NoContent);
        }

        private User CreateOwnerUserFromJson()
        {
            const string json = @"{""Id"":""47afc916-439c-4f57-82b3-0d47c9103390"",""TwitchUsername"":""tranquiliza"",""Email"":null,""PasswordHash"":"""",""PasswordSalt"":"""",""roles"":[],""EmailConfirmed"":true,""EmailConfirmationToken"":""721694ff-1032-462b-b9b9-aece212da208"",""ResetToken"":""484d94a6-ae84-476a-85fc-7a4afc55c030"",""ResetTokenExpiration"":""2020-07-13T01:06:38.3689257Z""}";

            return Serialization.Deserialize<User>(json);
        }

        [TestMethod]
        public async Task ModeratorShouldAuthorize()
        {
            // arrange
            const string moderatorName = "ChromaCarina";

            var suspensionRepositoryMock = new Mock<ISuspensionRepository>();
            suspensionRepositoryMock.Setup(x => x.GetSuspensionsForChannel(It.IsAny<string>())).ReturnsAsync(new List<Suspension>());
            var channelRepository = new Mock<IChannelRepository>();
            channelRepository.Setup(x => x.GetChannel(It.IsAny<string>())).ReturnsAsync(CreateChannelWithModerator(moderatorName));
            var context = new Mock<IApplicationContext>();
            context.Setup(x => x.User).Returns(CreateModeratorUserFromJson());
            var tagRepository = new Mock<ITagRepository>();
            var messageDispatcher = new Mock<IMessageDispatcher>();

            var service = new SuspensionService(suspensionRepositoryMock.Object, channelRepository.Object, tagRepository.Object, messageDispatcher.Object);

            // act
            var result = await service.GetAllSuspensionsAsync("Tranquiliza", context.Object).ConfigureAwait(false);

            // assert
            Assert.AreEqual(result.State, ResultState.NoContent);
        }

        private Channel CreateChannelWithModerator(string moderatorName)
        {
            var channel = new Channel("Tranquiliza", true);
            channel.TryAddModerator(moderatorName);
            return channel;
        }

        private User CreateModeratorUserFromJson()
        {
            const string json = @"{""Id"":""47afc916-439c-4f57-82b3-0d47c9103390"",""TwitchUsername"":""ChromaCarina"",""Email"":null,""PasswordHash"":"""",""PasswordSalt"":"""",""roles"":[],""EmailConfirmed"":true,""EmailConfirmationToken"":""721694ff-1032-462b-b9b9-aece212da208"",""ResetToken"":""484d94a6-ae84-476a-85fc-7a4afc55c030"",""ResetTokenExpiration"":""2020-07-13T01:06:38.3689257Z""}";

            return Serialization.Deserialize<User>(json);
        }

        [TestMethod]
        public async Task UserShouldNOTAuthorize()
        {
            // arrange
            var suspensionRepositoryMock = new Mock<ISuspensionRepository>();
            suspensionRepositoryMock.Setup(x => x.GetSuspensionsForChannel(It.IsAny<string>())).ReturnsAsync(new List<Suspension>());
            var channelRepository = new Mock<IChannelRepository>();
            channelRepository.Setup(x => x.GetChannel(It.IsAny<string>())).ReturnsAsync(new Channel("Tranquiliza", true));
            var context = new Mock<IApplicationContext>();
            context.Setup(x => x.User).Returns(CreateUserFromJson());
            var tagRepository = new Mock<ITagRepository>();
            var messageDispatcher = new Mock<IMessageDispatcher>();

            var service = new SuspensionService(suspensionRepositoryMock.Object, channelRepository.Object, tagRepository.Object, messageDispatcher.Object);

            // act
            var result = await service.GetAllSuspensionsAsync("Tranquiliza", context.Object).ConfigureAwait(false);

            // assert
            Assert.AreEqual(result.State, ResultState.AccessDenied);
        }

        private User CreateUserFromJson()
        {
            const string json = @"{""Id"":""47afc916-439c-4f57-82b3-0d47c9103390"",""TwitchUsername"":""UnknownUser"",""Email"":null,""PasswordHash"":"""",""PasswordSalt"":"""",""roles"":[],""EmailConfirmed"":true,""EmailConfirmationToken"":""721694ff-1032-462b-b9b9-aece212da208"",""ResetToken"":""484d94a6-ae84-476a-85fc-7a4afc55c030"",""ResetTokenExpiration"":""2020-07-13T01:06:38.3689257Z""}";

            return Serialization.Deserialize<User>(json);
        }
    }
}
