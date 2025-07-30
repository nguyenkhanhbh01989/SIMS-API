using SIMS.API.DTOs.Auth;
using SIMS.API.DTOs.User;
using SIMS.API.Repositories.Interfaces;

namespace SIMS.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IGoogleAuthService googleAuthService)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _googleAuthService = googleAuthService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            // 1. Xác thực Google ID Token để lấy email an toàn
            var verifiedEmail = await _googleAuthService.ValidateGoogleTokenAndGetEmailAsync(loginRequest.GoogleIdToken);

            if (string.IsNullOrEmpty(verifiedEmail))
            {
                throw new ApplicationException("Xác thực Google thất bại hoặc token không hợp lệ.");
            }

            // 2. Sử dụng email đã xác minh để tìm người dùng
            var user = await _userRepository.GetUserByEmailAsync(verifiedEmail);

            if (user == null)
            {
                throw new ApplicationException("Tài khoản của bạn chưa được cấp phép. Vui lòng liên hệ quản trị viên.");
            }

            if (!user.IsActive)
            {
                throw new ApplicationException("Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.");
            }

            // 3. Tạo JWT token của hệ thống và chuẩn bị phản hồi
            var token = _jwtTokenGenerator.GenerateToken(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName
            };

            return new LoginResponseDto
            {
                JwtToken = token,
                IsProfileIncomplete = string.IsNullOrEmpty(user.FullName),
                User = userDto
            };
        }
    }
}