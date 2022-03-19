ZoomMtg.preLoadWasm();
ZoomMtg.prepareWebSDK();
// loads language files, also passes any error messages to the ui
ZoomMtg.i18n.load('en-US');
ZoomMtg.i18n.reload('en-US');

ZoomMtg.setZoomJSLib('https://source.zoom.us/2.3.0/lib', '/av');
var apiKey = '';
var meetingNumber = '';
var leaveUrl = 'https://zoom.us';
var userName = 'mailsforbeni';
var userEmail = 'mailsforbeni@gmail.com'
var passWord = '';
function getSignature(role) {

    if (role == 1) {
        meetingNumber = $.trim($('#id').text());
        passWord = $.trim($('#password').text());
    }
    else {
        userName = $.trim($('#name').val());
        meetingNumber = $.trim($('#id').val());
        passWord = $.trim($('#password').val());
    }
    $.ajax({
        type: "GET",
        url: window.location.origin + "/StartZoom/GenerateSignature",
        dataType: "json",
        contentType: "application/json",
        data: { "meetingnumber": meetingNumber, 'role': role },
        success: function (data) {

            startMeeting(data);
        }
    });
}

function startMeeting(dt) {
    apiKey = dt[0];
    signature = dt[1];

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
