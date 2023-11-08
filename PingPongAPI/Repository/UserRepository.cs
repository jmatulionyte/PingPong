using System;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PingPongAPI.Data;
using PingPongAPI.Models.Dto;
using PingPongAPI.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PingPongAPI.Models;

namespace PingPongAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager; //identity service helper tool
        private readonly RoleManager<IdentityRole> _roleManager; //identity service helper tool

        private string secretKey;
        private readonly IMapper _mapper;
        //dependency injection
        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
            _mapper = mapper;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool isUserUnique(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null) //no user with that username
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            //check if there are user with same username and pw
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            //use usernamager to check pw validity
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            //user is not existant or pw does not fit username
            if (user == null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }
            //if found generate jwt token, using secret and key  token will be encrypted
            //secret key is used to validate if token is valid or not
            //secret key is used to authenticate wether this token was generated in this api
            var tokenHandler = new JwtSecurityTokenHandler();
            //convert secretKey string to byte array
            var key = Encoding.ASCII.GetBytes(secretKey);

            //get roles from roles table for user
            var roles = await _userManager.GetRolesAsync(user);

            //token descriptor, tell what are the claims in the token
            //claim identify users name, role, etc, custom claims, default cliam for user Id, role, etc,
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()) //would need foreach if many roles
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                //generates signning credential for token descriptor - how to encrypt
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //this token has type SecurityClass, so it cannot be passed as value?
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                //writetoken allogs to get token value 
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                //Role = roles.FirstOrDefault()
            };
            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.UserName,
                Email = registrationRequestDTO.UserName,
                NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
                FirstName = registrationRequestDTO.FirstName,
                LastName = registrationRequestDTO.LastName
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded) //if creation successful, only then assign authorization
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    { //custom situation, when no admin role is provided
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await _userManager.AddToRoleAsync(user, "admin");
                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == registrationRequestDTO.UserName);
                    return _mapper.Map<UserDTO>(userToReturn);
                }
                else
                {
                    //do smth????
                }
            }
            catch (Exception ex)
            {
                //do smth????
            }
            return new UserDTO();
        }

        public string GetNameFromTokenData(string id)
        {
            string fullName = _db.ApplicationUsers.Where(x => x.Id == id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
            return fullName;
        }
    }

}

