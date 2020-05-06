
using Bis_GUI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Http;
using Tutorial_3_Web_Service.Models;

namespace Tutorial_3_Web_Service.Controllers
{

    public class WebApiController : ApiController
    {
        //GET api/<controller>
        DataModel model = new DataModel();
        public int Get()
        {
            LogData log = new LogData();
            log.logNumEntries(model.getNumEntries());
            return model.getNumEntries();
        }

        public DataIntermed Post([FromBody] FilePathData fileValue)
        {
            LogData log = new LogData();
            DataIntermed dataInter = new DataIntermed();
            DataModel model = new DataModel();
            model.UpdateFilePath(fileValue.filePath, fileValue.indexToUpdate);
            model.GetValuesForEntry(fileValue.indexToUpdate, out dataInter.acct, out dataInter.pin, out dataInter.bal, out dataInter.fname, out dataInter.lname, out dataInter.filePath);
            log.logImageUpload(fileValue.filePath);
            return dataInter;
        }

        public DataIntermed Put([FromBody] DataIntermed dataIm)
        {
            LogData log = new LogData();
            DataModel model = new DataModel();
            model.updateUserEntry(dataIm.index, dataIm.fname, dataIm.lname, dataIm.acct, dataIm.pin, dataIm.bal);
            model.GetValuesForEntry(dataIm.index, out dataIm.acct, out dataIm.pin, out dataIm.bal, out dataIm.fname, out dataIm.lname, out dataIm.filePath);
            log.updateAccount(dataIm);
            return dataIm; 
        }
    }
    
}
