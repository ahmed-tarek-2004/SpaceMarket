using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.DataAccess.Services.Auth;
using Ecommerce.DataAccess.Services.Email;
using Ecommerce.DataAccess.Services.OTP;
using Ecommerce.DataAccess.Services.Token;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Shared.Bases;

using Microsoft.AspNetCore.Identity;

using Moq;

namespace DataAccess.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<AuthContext> _mockAuthContext;
        private readonly Mock<ITokenStoreService> _mockTokenStoreService;
        private readonly AuthService _authService;
        private readonly ResponseHandler _responseHandler;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IOTPService> _mockOTPService;

        public AuthServiceTests()
        {
            // Mock UserManager
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _mockTokenStoreService = new Mock<ITokenStoreService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockOTPService = new Mock<IOTPService>();
            _mockAuthContext = new Mock<AuthContext>();
            _responseHandler = new ResponseHandler();

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockAuthContext.Object,
                _mockEmailService.Object,
                _mockOTPService.Object,
                _responseHandler,
                _mockTokenStoreService.Object,
                null
            );
        }
        // TODO : We will use InMemoryDatabase
    }
}