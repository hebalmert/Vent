using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Vent.AccessData.Data;
using Vent.Backend.Controllers;
using Vent.Contracts.Events;
using Vent.Helpers;
using Vent.Shared.DTOs;
using Vent.Shared.Entities;
using Vent.Shared.Enum;
using Vent.Shared.ResponsesSec;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Vent.Tests;

public class AccountsControllerTests
{
    private readonly AccountsController _controller;
    private readonly DataContext _context;
    private readonly Mock<IUserHelper> _userHelperMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IEmailHelper> _emailHelperMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    public AccountsControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Aísla cada test
            .Options;

        _context = new DataContext(options);

        _userHelperMock = new Mock<IUserHelper>();
        _configurationMock = new Mock<IConfiguration>();
        _emailHelperMock = new Mock<IEmailHelper>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        _controller = new AccountsController(
            _context,
            _userHelperMock.Object,
            _configurationMock.Object,
            _emailHelperMock.Object,
            _publishEndpointMock.Object
        );
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnNoContent_WhenUserConfirmed()
    {
        var userId = Guid.NewGuid().ToString();
        var token = "test-token";

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Test User"
        };

        _userHelperMock.Setup(x => x.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        _userHelperMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.ConfirmEmailAsync(userId, token);

        Assert.IsType<NoContentResult>(result);

        _publishEndpointMock.Verify(pe => pe.Publish(It.Is<UserActivatedEvent>(
            u => u.Email == user.Email && u.FullName == user.FullName), default), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();
        var token = "test-token";

        _userHelperMock.Setup(x => x.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        var result = await _controller.ConfirmEmailAsync(userId, token);

        Assert.IsType<NotFoundResult>(result);

        _publishEndpointMock.Verify(pe => pe.Publish(It.IsAny<UserActivatedEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ShouldReturnBadRequest_WhenConfirmationFails()
    {
        var userId = Guid.NewGuid().ToString();
        var token = "invalid-token";

        var user = new User
        {
            Id = userId,
            Email = "fail@example.com",
            FullName = "Failing User"
        };

        _userHelperMock.Setup(x => x.GetUserAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        var identityError = IdentityResult.Failed(new IdentityError { Description = "Token inválido" });
        _userHelperMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(identityError);

        var result = await _controller.ConfirmEmailAsync(userId, token);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Token inválido", badRequest.Value!.ToString());

        _publishEndpointMock.Verify(pe => pe.Publish(It.IsAny<UserActivatedEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        var email = "admin@demo.com";
        var loginDto = new LoginDTO { Email = email, Password = "123456" };
        var userId = Guid.NewGuid().ToString();

        var user = new User
        {
            Id = userId,
            Email = email,
            FullName = "Admin Demo",
            Activo = true,
            CorporationId = 1,
            UserFrom = "Manager",
            PhotoUser = null,
            FirstName = "Admin",
            LastName = "Demo"
        };

        _context.Corporations.Add(new Corporation
        {
            CorporationId = 1,
            Name = "Demo Corp",
            Active = true,
            DateEnd = DateTime.Today.AddDays(30),
            ImagenId = "demo.png",
            Address = "123 Calle Ficticia",
            NroDocument = "123456789",
            Phone = "555-1234",
            CountryId = 1,
            SoftPlanId = 1,
            DateStart = DateTime.Today
        });

        _context.UserRoleDetails.Add(new UserRoleDetails
        {
            UserId = userId,
            UserType = UserType.Admin
        });

        await _context.SaveChangesAsync();

        _userHelperMock.Setup(u => u.LoginAsync(loginDto))
            .ReturnsAsync(SignInResult.Success);

        _userHelperMock.Setup(u => u.GetUserAsync(email))
            .ReturnsAsync(user);

        _configurationMock.Setup(c => c["jwtKey"])
            .Returns("thisIsASecretKeyOfAtLeast32CharsLong123!");

        var result = await _controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var token = Assert.IsType<TokenDTO>(okResult.Value);
        Assert.False(string.IsNullOrWhiteSpace(token.Token));
    }
}