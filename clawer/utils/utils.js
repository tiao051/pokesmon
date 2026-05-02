const axios = require('axios');

async function downloadImageAsBase64(url) {
    if (!url) return null;

    try {
        const response = await axios({
            url,
            method: 'GET',
            responseType: 'arraybuffer'
        });
        
        const base64 = Buffer.from(response.data, 'binary').toString('base64');
        const mimeType = response.headers['content-type'] || 'image/jpeg';
        
        return `data:${mimeType};base64,${base64}`;
    } catch (err) {
        console.error(`Error downloading image ${url}:`, err.message);
        return null;
    }
}

module.exports = {
    downloadImageAsBase64
};
