using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.Repositories;
using IntegrationModul.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationModul.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationController(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public ActionResult<Notification> CreateNotification([FromQuery] string receiverEmail, [FromQuery] string subject, [FromQuery] string body)
        {
            try
            {
                if (string.IsNullOrEmpty(subject))
                {
                    return BadRequest("Subject is required");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var notification = new Notification
                {
                    CreatedAt = DateTime.Now,
                    ReceiverEmail = receiverEmail,
                    Subject = subject,
                    Body = body,
                    SentAt = DateTime.Now,
                };
                var blNotification = _mapper.Map<BLNotification>(notification);
                var newNotification = _notificationRepository.AddNotification(blNotification);
                var createdNotification = _mapper.Map<Notification>(newNotification);
                return Ok(createdNotification);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with creating the notification");
            }
        }
        [HttpPut("[action]/{id}")]
        public ActionResult<Notification> UpdateNotification(int id, [FromQuery] string receiverEmail, [FromQuery] string subject, [FromQuery] string body)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var blNotification = _notificationRepository.GetNotificationByID(id);
                if (blNotification == null)
                {
                    return NotFound($"Notification for update with ID: {id} is not found");
                }
                blNotification.ReceiverEmail = receiverEmail;
                blNotification.Subject = subject;
                blNotification.Body = body;
                var updatedNotification = _notificationRepository.UpdateNotification(id, blNotification);
                var notificationn = _mapper.Map<Notification>(updatedNotification);
                return Ok(notificationn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with updating the notification.");
            }
        }

        [HttpDelete("[action]/{id}")]
        public ActionResult DeleteNotification(int id)
        {
            try
            {
                var blNotification = _notificationRepository.GetNotificationByID(id);
                if (blNotification == null)
                {
                    return NotFound($"Notification with ID: {id} is not found");
                }
                _notificationRepository.DeleteNotification(id);
                return Ok(blNotification);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with deleting the tag, tag with that id is connect with other table");
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Notification>> GetAllNotifications()
        {
            try
            {
                var blNotification = _notificationRepository.GetAllNotifications();
                if (blNotification == null)
                {
                    return NotFound($"There are no notifications in the database");
                }
                var notification = _mapper.Map<IEnumerable<BLNotification>>(blNotification);
                return Ok(notification);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching all notifications.");
            }
        }
        [HttpGet("[action]{id}")]
        public ActionResult<Notification> GetNotificationByID(int id)
        {
            try
            {
                var blNotification = _notificationRepository.GetNotificationByID(id);
                if (blNotification == null)
                {
                    return NotFound($"Notification with ID: {id} is not found");
                }
                var notification = _mapper.Map<Notification>(blNotification);
                return Ok(notification);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching notification");
            }
        }
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Notification>> SearchNotificationsBySpecificPartOfSubject(string part)
        {
            try
            {
                var blNotifications = _notificationRepository.GetAllNotifications().Where(x => x.Subject.ToLower().Contains(part.ToLower()));
                if (blNotifications == null)
                {
                    return NotFound($"Notifications with that part is not found");
                }
                var notifications = _mapper.Map<IEnumerable<Notification>>(blNotifications);
                return Ok(notifications);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem with fetching notifications");
            }
        }
    }
}
