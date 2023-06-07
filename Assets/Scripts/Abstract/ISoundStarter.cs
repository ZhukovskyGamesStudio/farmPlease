public interface ISoundStarter 
{
    public void PlaySound(int soundIndex) {
        Audio.Instance.PlaySound((Sounds) soundIndex);
    }
}
