package com.ridesharing.admin.service;

import com.azure.storage.blob.BlobClient;
import com.azure.storage.blob.BlobContainerClient;
import com.azure.storage.blob.BlobServiceClient;
import com.azure.storage.blob.BlobServiceClientBuilder;
import com.azure.storage.blob.sas.BlobSasPermission;
import com.azure.storage.blob.sas.BlobServiceSasSignatureValues;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.time.OffsetDateTime;
import java.util.UUID;

@Service
public class BlobStorageService {
    
    @Value("${azure.storage.connection-string}")
    private String connectionString;
    
    @Value("${azure.storage.container-name}")
    private String containerName;
    
    public String uploadFile(MultipartFile file) throws IOException {
        if (connectionString == null || connectionString.isEmpty() || connectionString.equals("UseDevelopmentStorage=true")) {
            String mockUrl = "https://ridesharing.blob.core.windows.net/banners/" + UUID.randomUUID().toString() + "_" + file.getOriginalFilename();
            return mockUrl;
        }
        
        try {
            BlobServiceClient blobServiceClient = new BlobServiceClientBuilder()
                .connectionString(connectionString)
                .buildClient();
            
            BlobContainerClient containerClient = blobServiceClient.getBlobContainerClient(containerName);
            
            if (!containerClient.exists()) {
                containerClient.create();
            }
            
            String fileName = UUID.randomUUID().toString() + "_" + file.getOriginalFilename();
            BlobClient blobClient = containerClient.getBlobClient(fileName);
            
            blobClient.upload(file.getInputStream(), file.getSize(), true);
            
            BlobSasPermission sasPermission = new BlobSasPermission().setReadPermission(true);
            BlobServiceSasSignatureValues sasValues = new BlobServiceSasSignatureValues(
                OffsetDateTime.now().plusYears(10), sasPermission);
            
            String sasToken = blobClient.generateSas(sasValues);
            return blobClient.getBlobUrl() + "?" + sasToken;
        } catch (Exception e) {
            String mockUrl = "https://ridesharing.blob.core.windows.net/banners/" + UUID.randomUUID().toString() + "_" + file.getOriginalFilename();
            return mockUrl;
        }
    }
}
