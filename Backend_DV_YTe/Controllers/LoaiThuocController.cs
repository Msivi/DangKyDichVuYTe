using AutoMapper;
using Backend_DV_YTe.Entity;
using Backend_DV_YTe.Model;
using Backend_DV_YTe.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Authorization;

namespace Backend_DV_YTe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "QuanLy")]
    public class LoaiThuocController : ControllerBase
    {
        private readonly ILoaiThuocRepository _loaiThuocRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public LoaiThuocController(ILoaiThuocRepository LoaiThuocRepository, IMapper mapper, IDistributedCache distributedCache)
        {
            _loaiThuocRepository = LoaiThuocRepository;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("/api/[controller]/get-loai-thuoc-by-id")]
        public async Task<ActionResult<LoaiThuocEntity>> GetLoaiThuocById(int id)
        {
            try
            {
                var entity = await _loaiThuocRepository.GetLoaiThuocById(id);

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpGet]
        [Route("/api/[controller]/get-all-loai-thuoc")]
        public async Task<ActionResult<ICollection<LoaiThuocEntity>>> GetAllLoaiThuoc()
        {
            try
            {
                var entity = await _loaiThuocRepository.GetAllLoaiThuoc();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                dynamic result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }

        //[HttpGet]
        //[Route("/api/[controller]/search-loai-thuoc")]
        //public async Task<ActionResult<IEnumerable<LoaiThuocEntity>>> SearchLoaiThuoc(string searchKey)
        //{
        //    try
        //    {
        //        var LoaiThuocList = await _loaiThuocRepository.SearchLoaiThuoc(searchKey);
        //        if (!LoaiThuocList.Any())
        //        {
        //            return BadRequest("Not found!");
        //        }
        //        return Ok(LoaiThuocList);
        //    }
        //    catch (Exception ex)
        //    {
        //        dynamic result = new BaseResponseModel<string>(
        //            statusCode: StatusCodes.Status500InternalServerError,
        //            code: "Failed!",
        //            message: ex.Message);
        //        return BadRequest(result);
        //    }
        //}

        [HttpPost]
        [Route("/api/[controller]/create-loai-thuoc")]
        public async Task<ActionResult<string>> CreateLoaiThuoc(LoaiThuocModel model)
        {
            try
            {
                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<LoaiThuocEntity>(model);
                mapEntity.CreateBy = userId;
                var result = await _loaiThuocRepository.CreateLoaiThuoc(mapEntity);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status201Created,
                    code: "Success!",
                    data: result));
            }
            catch (Exception ex)
            {
                dynamic result;
                result = new BaseResponseModel<string>(
                   statusCode: StatusCodes.Status500InternalServerError,
                   code: "Failed!",
                   message: ex.Message);
                return BadRequest(result);
            }
        }
        [HttpPut]
        [Route("/api/[controller]/update-loai-thuoc")]
        public async Task<ActionResult> UpdateLoaiThuoc(int id, LoaiThuocModel entity)
        {
            try
            {

                byte[] userIdBytes = await _distributedCache.GetAsync("UserId");// Lấy giá trị UserId từ Distributed Cache
                int userId = BitConverter.ToInt32(userIdBytes, 0);

                var mapEntity = _mapper.Map<LoaiThuocEntity>(entity);
                mapEntity.CreateBy = userId;
                await _loaiThuocRepository.UpdateLoaiThuoc(id, mapEntity);

                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Updated successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Error",
                    message: ex.Message));
            }
        }
        [HttpDelete]
        [Route("/api/[controller]/delete-loai-thuoc")]
        public async Task<ActionResult<LoaiThuocEntity>> DeleteLoaiThuoc(int keyId)
        {

            try
            {

                await _loaiThuocRepository.DeleteLoaiThuoc(keyId, false);
                return Ok(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status200OK,
                    code: "Success!",
                    data: "Delete successfully!"));
            }
            catch (Exception ex)
            {

                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Error",
                    message: ex.Message));
            }
        }

        //====================
        [HttpPost]
        [Route("/api/[controller]/send-email")]
        public IActionResult SendEmailToUser([FromBody] EmailModel model)
        {
            try
            {
                string recipientEmail = model.RecipientEmail;
                string subject = model.Subject;
                string message = model.Message;

                // Gọi phương thức gửi email
                _loaiThuocRepository.SendEmail(recipientEmail, subject, message);

                // Trả về kết quả thành công
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có lỗi xảy ra
                return BadRequest(ex.Message);
            }
        }
    }
}
