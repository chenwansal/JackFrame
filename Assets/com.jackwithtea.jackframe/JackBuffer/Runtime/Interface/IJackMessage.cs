
namespace JackBuffer {

    public interface IJackMessage<T> {

        void WriteTo(byte[] dst, ref int offset);
        byte[] ToBytes();
        int GetEvaluatedSize(out bool isCertain);

    }

}