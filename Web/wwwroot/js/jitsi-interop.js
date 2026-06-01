(function () {
    let api = null;
    let scriptPromise = null;

    function loadJitsiApi(domain) {
        if (window.JitsiMeetExternalAPI) {
            return Promise.resolve();
        }

        if (scriptPromise) {
            return scriptPromise;
        }

        scriptPromise = new Promise((resolve, reject) => {
            const script = document.createElement("script");
            script.src = `https://${domain}/external_api.js`;
            script.async = true;
            script.onload = resolve;
            script.onerror = () => reject(new Error("Could not load the Jitsi Meet API."));
            document.head.appendChild(script);
        });

        return scriptPromise;
    }

    window.studyConnectVideo = {
        async start(options) {
            const domain = options.domain || "meet.jit.si";
            const container = document.getElementById(options.containerId);

            if (!container) {
                throw new Error(`Video container '${options.containerId}' was not found.`);
            }

            if (!options.roomName) {
                throw new Error("A room name is required to start the video call.");
            }

            await this.stop();
            await loadJitsiApi(domain);

            container.innerHTML = "";

            api = new window.JitsiMeetExternalAPI(domain, {
                roomName: options.roomName,
                parentNode: container,
                width: "100%",
                height: "100%",
                userInfo: {
                    email: options.email || "",
                    displayName: options.displayName || "StudyConnect User"
                },
                configOverwrite: {
                    prejoinPageEnabled: true,
                    startWithAudioMuted: false,
                    startWithVideoMuted: false,
                    disableDeepLinking: true,
                    enableWelcomePage: false,
                    ...options.configOverwrite
                },
                interfaceConfigOverwrite: {
                    SHOW_CHROME_EXTENSION_BANNER: false,
                    MOBILE_APP_PROMO: false,
                    TOOLBAR_BUTTONS: [
                        "microphone",
                        "camera",
                        "desktop",
                        "fullscreen",
                        "fodeviceselection",
                        "hangup",
                        "chat",
                        "settings",
                        "raisehand",
                        "videoquality",
                        "filmstrip",
                        "tileview",
                        "shortcuts"
                    ],
                    ...options.interfaceConfigOverwrite
                }
            });

            api.addListener("readyToClose", () => {
                window.dispatchEvent(new CustomEvent("studyconnect-video-closed"));
            });

            return true;
        },

        async stop() {
            if (api) {
                api.dispose();
                api = null;
            }
        }
    };
})();
