﻿using IsTableBusy.Core;
using IsTableBusy.Core.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace IsTableBusy.App.Api.Controllers
{
    
    public class PlacesController : ApiController
    {
        private TableInPlaceReader tableInPlaceReader;
        public PlacesController(TableInPlaceReader tableInPlaceReader)
        {
            this.tableInPlaceReader = tableInPlaceReader;
        }


        [Route("places/{place}/tables")]
        public IEnumerable<TableViewModel> GetTablesForPlace(string place)
        {
            var result =  this.tableInPlaceReader.Read(place);
            return result;
        }
    }
}
