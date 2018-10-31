using System.Text;
using LineOS.NTFS.Model.Enums;

namespace LineOS.NTFS.Model.Attributes
{
    public class AttributeVolumeName : Attribute
    {
        public string VolumeName { get; set; }

        public override AttributeResidentAllow AllowedResidentStates
        {
            get
            {
                return AttributeResidentAllow.Resident;
            }
        }

        internal override void ParseAttributeResidentBody(byte[] data, int maxLength, int offset)
        {
            base.ParseAttributeResidentBody(data, maxLength, offset);

            // Debug.Assert(maxLength >= ResidentHeader.ContentLength);

            VolumeName = Encoding.Unicode.GetString(data, offset, (int)ResidentHeader.ContentLength);
        }
    }
}