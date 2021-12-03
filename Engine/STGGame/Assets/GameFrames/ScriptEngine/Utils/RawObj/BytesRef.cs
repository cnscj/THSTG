namespace XLua
{
    public class BytesRef : RawObject
    {
        byte[] data;

        public BytesRef(byte[] data)
        {
            this.data = data;
        }

        public BytesRef(int length)
        {
            data = new byte[length];
        }

        public object Target
        {
            get
            {
                return data;
            }
        }

        public byte this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }
    }
}