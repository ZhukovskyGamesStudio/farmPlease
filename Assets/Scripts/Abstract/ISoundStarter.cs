public interface ISoundStarter 
{
    public void PlaySound(int soundIndex) {
        AudioManager.instance.PlaySound((Sounds) soundIndex);
    }
}
