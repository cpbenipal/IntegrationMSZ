ZoomMtg.preLoadWasm();
ZoomMtg.prepareWebSDK();
// loads language files, also passes any error messages to the ui
ZoomMtg.i18n.load('en-US');
ZoomMtg.i18n.reload('en-US');

ZoomMtg.setZoomJSLib('https://source.zoom.us/2.3.0/lib', '/av');

// setup your signature endpoint here: https://github.com/zoom/meetingsdk-sample-signature-node.js
var signatureEndpoint = '';
var apiKey = 'n26GV8IFSMWdg0GDNoDz2Q';
var meetingNumber = '';
 
var leaveUrl = 'https://zoom.us';
var userName = 'mailsforbeni';
var userEmail = 'mailsforbeni@gmail.com'
var passWord = '';
// pass in the registrant's token if your meeting or webinar requires registration. More info here:
// Meetings: https://marketplace.zoom.us/docs/sdk/native-sdks/web/client-view/meetings#join-registered
// Webinars: https://marketplace.zoom.us/docs/sdk/native-sdks/web/client-view/webinars#join-registered
var registrantToken = '';


function getSignature(role) {
   
    meetingNumber = $.trim($('#id').text());
    passWord = $.trim($('#password').text());
    $.ajax({
        type: "GET",
        url: window.location.origin + "/StartZoom/GenerateSignature",
        dataType: "json",
        contentType: "application/json",
        data: { "meetingnumber": meetingNumber, 'role': role },
        success: function (data) {
            ;
            startMeeting(data);
        }
    });
}


function startMeeting(signature) {
    
    document.getElementById('zmmtg-root').style.display = 'block'

    ZoomMtg.init({
        leaveUrl: leaveUrl,
        success: (success) => {
            console.log(success)
            ZoomMtg.join({
                signature: signature,
                apiKey: apiKey,
                meetingNumber: meetingNumber,
                userName: userName,
                userEmail: userEmail,
                passWord: passWord,
                success: (success) => {
                    console.log(success);
                },
                error: (error) => {
                    console.log(error);
                },
            });
        },
        error: (error) => {
            console.log(error)
        }
    })
}
