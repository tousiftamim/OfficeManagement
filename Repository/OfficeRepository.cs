using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Office_Management.Models;
using Office_Management.Models.ViewModel;

namespace Office_Management.Repository
{
    public class OfficeRepository
    {
        DatabaseContex dbContex = new DatabaseContex();

        public OfficeInfo Get(int id)
        {
            return dbContex.OfficeInfos.Find(id);
        }

        public List<OfficeInfo> GetAll()
        {
            return dbContex.OfficeInfos.ToList();
        }
        public List<OfficeInfo> GetAllByOfficeOwnerId(int officeOwnerId)
        {
            var officelist = dbContex.OfficeInfos.Where(_ => _.OwnerId == officeOwnerId).ToList();
            return officelist;
        }
    }
}