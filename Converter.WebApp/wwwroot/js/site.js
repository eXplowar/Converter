// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";

var savedToken = getTokenFromCookie();
var connection = null;

if (savedToken) {
    connectToSignalR(savedToken);
}

/**
 * Set connection with web app using SignalR
 * @param {any} token User identifier
 */
function connectToSignalR(token) {
    if (connection) {
        return;
    }

    connection = new signalR
        .HubConnectionBuilder()
        .withUrl("/repositoryHub")
        .withAutomaticReconnect()
        .build();

    connection.serverTimeoutInMilliseconds = 120000;
    connection.keepAliveIntervalInMilliseconds = 60000;

    const startConnection = async () => {
        await connection.start().then(() => {
            connection.send("RegisterClient", token);
        }).catch(error => {
            console.error('Error:', error);
        });
    }

    connection.on("CompletedConversion", (hash, token) => {
        downloadFile(hash, token);
    });

    connection.onclose(async () => {
        console.log('Connecition closed');
        await setTimeout(startConnection, 1000);
    });

    startConnection();
}

/**
 * Submit upload
 * @param {any} form Form data
 */
async function submitUpload(form) {
    const formData = new FormData(form);

    if (formData.get('fileInput').size === 0) {
        alert('No selected file!');
        return;
    }

    var params = {
        method: 'POST',
        body: formData,
        headers: {
            'Accept': 'application/json, text/plain'
        }
    };

    if (savedToken) {
        params.headers = { 'token': savedToken };
    }

    form.elements.submitButton.disabled = true;

    let response = await fetch(form.action, params);
    let result = await response.json();

    if (!response.ok) {
        console.error('Error:', result);
        return;
    }

    form.elements.submitButton.disabled = false;
    form.elements.result.innerText = 'The file was uploaded successfully. Wait for the download to start...';

    saveToken(result.token);
    connectToSignalR(result.token);
}

/**
 * Download file
 * @param {any} hash File identifier
 * @param {any} token User identifier
 */
async function downloadFile(hash, token) {
    await fetch(`Files/Download?file=${hash}`, {
        method: 'GET',
        headers: {
            'token': token,
        }
    })
        .then(async response => ({
            filename: getCorrectFileName(response.headers.get('content-disposition')),
            blob: await response.blob()
        }))
        .then(result => {
            let link = document.createElement("a");
            link.href = window.URL.createObjectURL(result.blob);
            link.download = result.filename;
            link.click();

            document.getElementsByName('result')[0].value = '';
        }).catch(error => {
            console.error('Error:', error);
        });
}

/**
 * Return correct decoded filename from header
 * @param {any} header Header
 */
function getCorrectFileName(header) {
    let contentDispostion = header.split(';');
    const fileNameToken = `filename*=UTF-8''`;

    let encodedFileName = contentDispostion
        .find(s => s.includes(fileNameToken))
        .replace(fileNameToken, '')
        .trim();

    let dencodedFileName = decodeURIComponent(encodedFileName);

    return dencodedFileName;
};

/**
 * Save token as global variable and also in cookie
 * @param {any} token User identifier
 */
function saveToken(token) {
    if (getTokenFromCookie() !== token) {
        setTokenInCookie(token);
    }

    if (!savedToken) {
        savedToken = token;
    }
}

/**
 * Save token in cookie
 * @param {any} token
 */
function setTokenInCookie(token) {
    document.cookie = `token=${token}`;
}

/** Return token from cookie if exist or return undefined */
function getTokenFromCookie() {
    let token = document.cookie
        .split('; ')
        .find(row => row.startsWith('token='))
        ?.split('=')[1];

    return token;
}

/**
 * Delete tokenfrom cookie
 * @param {any} name Cookie key name
 */
function deleteTokenFromCookie() {
    document.cookie = 'token=;max-age=-1';
    return 'Token removed';
}
