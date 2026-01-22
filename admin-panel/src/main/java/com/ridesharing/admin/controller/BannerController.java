package com.ridesharing.admin.controller;

import com.ridesharing.admin.entity.Banner;
import com.ridesharing.admin.repository.BannerRepository;
import com.ridesharing.admin.service.BlobStorageService;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;
import java.time.LocalDateTime;
import java.util.List;

@Controller
@RequestMapping("/banners")
public class BannerController {
    
    private final BannerRepository bannerRepository;
    private final BlobStorageService blobStorageService;
    
    public BannerController(BannerRepository bannerRepository, BlobStorageService blobStorageService) {
        this.bannerRepository = bannerRepository;
        this.blobStorageService = blobStorageService;
    }
    
    @GetMapping
    public String listBanners(Model model) {
        model.addAttribute("currentPage", "banners");
        List<Banner> banners = bannerRepository.findAll();
        model.addAttribute("banners", banners);
        model.addAttribute("newBanner", new Banner());
        return "banners";
    }
    
    @PostMapping("/upload")
    public String uploadBanner(@RequestParam("file") MultipartFile file,
                              @RequestParam("targetUrl") String targetUrl,
                              @RequestParam(value = "isActive", defaultValue = "true") Boolean isActive,
                              @RequestParam(value = "displayOrder", defaultValue = "0") Integer displayOrder) throws IOException {
        
        String imageUrl = blobStorageService.uploadFile(file);
        
        Banner banner = new Banner();
        banner.setImageUrl(imageUrl);
        banner.setTargetUrl(targetUrl);
        banner.setIsActive(isActive);
        banner.setDisplayOrder(displayOrder);
        banner.setCreatedAt(LocalDateTime.now());
        
        bannerRepository.save(banner);
        
        return "redirect:/banners";
    }
    
    @PostMapping("/{id}/toggle")
    public String toggleBanner(@PathVariable Integer id) {
        Banner banner = bannerRepository.findById(id)
            .orElseThrow(() -> new RuntimeException("Banner not found"));
        
        banner.setIsActive(!banner.getIsActive());
        bannerRepository.save(banner);
        
        return "redirect:/banners";
    }
    
    @PostMapping("/{id}/delete")
    public String deleteBanner(@PathVariable Integer id) {
        bannerRepository.deleteById(id);
        return "redirect:/banners";
    }
}
