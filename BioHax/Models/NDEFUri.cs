using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.ComponentModel.DataAnnotations.Schema;
using BioHax.Models;

namespace BioHax.Models
{
    public class NDEFUri : Service
    {
        public NDEFUri()
        {
            this.Record = new Record();
        }

        public void SaveRecord(string URI, int identiferCode)
        {
            this.Record.identifierCode = 0xD1;
            this.Record.recordTypeLength = 0x01;
            this.Record.recordLength = (byte)(Encoding.UTF8.GetByteCount(URI) - 1);
            this.Record.URIIdentifier = 0x55;
            this.Record.URIRecordType = (byte)identiferCode;
            this.Record.URI = Encoding.ASCII.GetBytes(URI);
        }

        public virtual Record Record { get; set; }
    }
}
