using AutoMapper;
using BusinessLayer.BLModels;
using BusinessLayer.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories
{
    public interface INotificationRepository
    {
        IEnumerable<BLNotification> GetAllNotifications();
        BLNotification GetNotificationByID(int id);
        BLNotification AddNotification(BLNotification notification);
        BLNotification UpdateNotification(int id, BLNotification notification);
        void DeleteNotification(int id);
    }

    public class NotificationRepository : INotificationRepository 
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;

        public NotificationRepository(RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public BLNotification AddNotification(BLNotification notification)
        {
            var newDbNotificaion = _mapper.Map<Notification>(notification);
            newDbNotificaion.Id = 0;
            _dbContext.Notifications.Add(newDbNotificaion);
            _dbContext.SaveChanges();
            var newBlNotificaion = _mapper.Map<BLNotification>(notification);
            return newBlNotificaion;
        }

        public BLNotification UpdateNotification(int id, BLNotification notification)
        {
            var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            if (dbNotification == null) 
            {
                throw new InvalidOperationException("Notification not found");
            }
            _mapper.Map(notification, dbNotification);
            _dbContext.SaveChanges();
            var updatedBlNotification = _mapper.Map<BLNotification>(dbNotification);
            return updatedBlNotification;
        }

        public void DeleteNotification(int id)
        {
            var notification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            if (notification == null)
            {
                throw new InvalidOperationException("Notification not found");
            }
            _dbContext.Notifications.Remove(notification);
            _dbContext.SaveChanges();
        }

        public IEnumerable<BLNotification> GetAllNotifications()
        {
            var dbNotifications = _dbContext.Notifications;
            var blNotifications = _mapper.Map<IEnumerable<BLNotification>>(dbNotifications);
            return blNotifications;
        }

        public BLNotification GetNotificationByID(int id)
        {
            var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
            var blNotification = _mapper.Map<BLNotification>(dbNotification);
            return blNotification;
        }
    }
}
