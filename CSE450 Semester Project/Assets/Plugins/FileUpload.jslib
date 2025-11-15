mergeInto(LibraryManager.library, {
    OpenFileUploadWeb: function (gameObjectName, methodName) {
        // you MUST parse from UTF8 pointer to string (apparently)
        var gameObjectName = UTF8ToString(gameObjectName);
        var methodName = UTF8ToString(methodName);
        var unitycanvas = document.getElementById('unity-canvas');

        if(!document.getElementById('UploadFileInput')) {
            var input = document.createElement("input");
            input.setAttribute('type', 'file');
            input.setAttribute('id', 'UploadFileInput');
            input.style.visibility = 'hidden';
            input.style.display = 'none';

            input.onclick = function (event) {
                this.value = null;
                var element = document.getElementById('UploadFileInput');
                element.parentNode.removeChild(element);
                unitycanvas.removeEventListener('click', OpenFileDialog, false);
            };

            input.onchange = function (event) {
                var file = event.target.files[0];
                if (!file) { return; }

                var reader = new FileReader();
                reader.onload = function (e) {
                    var contents = new Uint8Array(e.target.result);

                    // Create a virtual path
                    var path = "/uploaded/" + file.name;
                    try {
                        FS.mkdir("/uploaded");
                    } catch (e) {} // ignore if exists

                    // Write file to upload folder, send message to Unity script
                    FS.writeFile(path, contents);
                    SendMessage(gameObjectName, methodName, path);
                };
                reader.readAsArrayBuffer(file);

                // return focus to unity
                unitycanvas.focus();
            };

            document.body.appendChild(input);
        }

        var OpenFileDialog = function() {
            console.log("OpenFileDialog");
            document.getElementById('UploadFileInput').click();
        };
        unitycanvas.addEventListener('click', OpenFileDialog, false);
    }
});
