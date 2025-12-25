export function stopSound() {
    if (window.currentSound) {
        window.currentSound.pause();
        window.currentSound.currentTime = 0;
    }
}

export function playSound(name) {
    if (window.currentSound) {
        window.currentSound.pause();
        window.currentSound.currentTime = 0;
    }

    var audio = new Audio('media/' + name + '.mp3');
    window.currentSound = audio;
    audio.play();
}