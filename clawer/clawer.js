require('dotenv').config();
const connectDB = require('./utils/connectDB');
const axios = require('axios');
const Item = require('./models/Item');
const { SEARCH_URL, DETAIL_URL, IMAGE_URL } = require('./constants/endpoint');
const { downloadImageAsBase64, getSearchPayload, getSearchHeaders } = require('./utils/utils');

async function scrapeData() {
    console.log(`Starting to scrape data...`);
    
    let from = 0;
    const size = 24;
    let hasNextPage = true;
    
    while (hasNextPage) {
        console.log(`Fetching from offset: ${from}`);
        
        try {
            const response = await axios.post(SEARCH_URL, getSearchPayload(from, size), {
                headers: getSearchHeaders()
            });
            
            const resultsData = response.data.results[0];
            const items = resultsData.results;
            const totalResults = resultsData.totalResults;
            
            console.log(`Found ${items.length} items. Total results available: ${totalResults}`);
            
            if (items.length === 0) {
                hasNextPage = false;
                break;
            }
            
            await Promise.all(items.map(async (item) => {
                const productId = item.productId;
                console.log(`Processing product ID: ${productId}`);
                
                try {
                    const existingItem = await Item.findOne({ productId: productId });
                    if (existingItem) {
                        console.log(`Product ID ${productId} already exists in DB. Skipping...`);
                        return;
                    }

                    const detailUrl = DETAIL_URL.replace('[PRODUCT_ID]', productId);
                    const detailResponse = await axios.get(detailUrl);
                    const detailData = detailResponse.data;
                    
                    const imageUrl = IMAGE_URL.replace('${PRODUCT_ID}', productId);
                    const imageBase64 = await downloadImageAsBase64(imageUrl);
                    
                    const itemDataToSave = {
                        ...item,
                        ...detailData,
                        imageUrl,
                        imageBase64
                    };
                    
                    await Item.findOneAndUpdate(
                        { productId: productId },
                        itemDataToSave,
                        { upsert: true, returnDocument: 'after' }
                    );
                    console.log(`Saved product ID: ${productId}`);
                } catch (err) {
                    console.error(`Failed to process product ID: ${productId}`, err.message);
                }
            }));
            
            from += size;
            
            if (from >= totalResults) {
                hasNextPage = false;
            }
            
        } catch (error) {
            console.error(`Error fetching search page at offset ${from}:`, error.message);
            hasNextPage = false;
        }
    }
    
    console.log(`Finished scraping data.`);
}

async function run() {
    await connectDB();
    await scrapeData();
    console.log('Scraping complete.');
    process.exit(0);
}

run();
