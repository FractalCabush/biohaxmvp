using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BioHax.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Record
    {
        public int RecordID { get; set; }

        public byte identifierCode { get; set; }
        public byte recordTypeLength { get; set; }
        public byte recordLength { get; set; }
        public byte URIRecordType { get; set; }
        public byte URIIdentifier { get; set; }
        public byte[] URI { get; set; }

        public string FormattedURI
        {
            get
            {
                return Encoding.UTF8.GetString(this.URI);
            }
        }

        public string FormatRecordType
        {
            get
            {
                return this.URIRecordType.ToString();
            }
        }
        public string FormatURIIdentifier
        {
            get
            {
                return this.URIIdentifier.ToString();
            }
        }
    }
}
