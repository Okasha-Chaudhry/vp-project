// Jitsi Meet Interop Functions
let jitsiApi = null;

window.initializeJitsiMeeting = function(options) {
    const container = document.getElementById(options.parentNode);
    
    if (!container) {
        console.error('Jitsi container not found');
        return;
    }

    // Wait for JitsiMeetExternalAPI to be available
    const waitForJitsi = setInterval(() => {
        if (window.JitsiMeetExternalAPI) {
            clearInterval(waitForJitsi);
            
            try {
                jitsiApi = new window.JitsiMeetExternalAPI(
                    new URL(options.configOverwrite?.serverUrl || 'https://meet.jit.si').hostname,
                    {
                        roomName: options.roomName,
                        width: options.width,
                        height: options.height,
                        parentNode: options.parentNode,
                        userInfo: {
                            email: options.userInfo?.email || '',
                            displayName: options.userInfo?.displayName || 'User'
                        },
                        configOverwrite: options.configOverwrite || {},
                        interfaceConfigOverwrite: options.interfaceConfigOverwrite || {}
                    }
                );

                // Add event listeners
                jitsiApi.addEventListener('videoConferenceJoined', onVideoConferenceJoined);
                jitsiApi.addEventListener('videoConferenceLocked', onVideoConferenceLocked);
                jitsiApi.addEventListener('videoConferenceLeft', onVideoConferenceLeft);
                jitsiApi.addEventListener('readyToClose', onReadyToClose);
                jitsiApi.addEventListener('participantJoined', onParticipantJoined);
                jitsiApi.addEventListener('participantLeft', onParticipantLeft);
                jitsiApi.addEventListener('displayNameChanged', onDisplayNameChanged);
                jitsiApi.addEventListener('participantKickedOut', onParticipantKickedOut);
                jitsiApi.addEventListener('audioAvailabilityChanged', onAudioAvailabilityChanged);
                jitsiApi.addEventListener('videoAvailabilityChanged', onVideoAvailabilityChanged);

                console.log('Jitsi Meeting initialized successfully');
            } catch (error) {
                console.error('Error initializing Jitsi:', error);
            }
        }
    }, 100);

    // Set timeout to stop waiting after 10 seconds
    setTimeout(() => {
        clearInterval(waitForJitsi);
        if (!jitsiApi) {
            console.error('Jitsi API failed to load');
        }
    }, 10000);
};

window.disposeJitsiMeeting = function() {
    if (jitsiApi) {
        try {
            jitsiApi.dispose();
            jitsiApi = null;
            console.log('Jitsi Meeting disposed');
        } catch (error) {
            console.error('Error disposing Jitsi:', error);
        }
    }
};

// Event handlers
function onVideoConferenceJoined() {
    console.log('Video conference joined');
}

function onVideoConferenceLocked() {
    console.log('Video conference locked');
}

function onVideoConferenceLeft() {
    console.log('Video conference left');
}

function onReadyToClose() {
    console.log('Ready to close');
}

function onParticipantJoined(participant) {
    console.log('Participant joined:', participant);
}

function onParticipantLeft(participant) {
    console.log('Participant left:', participant);
}

function onDisplayNameChanged(data) {
    console.log('Display name changed:', data);
}

function onParticipantKickedOut(data) {
    console.log('Participant kicked out:', data);
}

function onAudioAvailabilityChanged(data) {
    console.log('Audio availability changed:', data);
}

function onVideoAvailabilityChanged(data) {
    console.log('Video availability changed:', data);
}
