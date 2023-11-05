using System;
using UnityEngine;

namespace com.NW84P
{
    [CreateAssetMenu(fileName = "PhoneDoneData", menuName = "ScriptableObjects/PhoneDoneData", order = 1)]
    public class PhoneDoneData : ScriptableObject
    {
        [SerializeField]
        private AudioClip _loveYouAudioClip;

        [SerializeField]
        private AudioClip _dogAudioClip;

        [SerializeField]
        private AudioClip _musicAudioClip;

        [SerializeField]
        private Sprite _loveYouSprite;

        [SerializeField]
        private Sprite _dogSprite;

        [SerializeField]
        private Sprite _musicSprite;

        public Tuple<AudioClip, Sprite>[] Data =>
            new Tuple<AudioClip, Sprite>[] {
                new(_loveYouAudioClip, _loveYouSprite),
                new(_dogAudioClip, _dogSprite),
                new(_musicAudioClip, _musicSprite)
            };
    }
}
