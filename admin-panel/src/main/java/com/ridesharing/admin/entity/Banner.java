package com.ridesharing.admin.entity;

import jakarta.persistence.*;
import lombok.Data;
import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "Banners")
public class Banner {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "ID")
    private Integer id;
    
    @Column(name = "ImageUrl")
    private String imageUrl;
    
    @Column(name = "TargetUrl")
    private String targetUrl;
    
    @Column(name = "IsActive")
    private Boolean isActive;
    
    @Column(name = "DisplayOrder")
    private Integer displayOrder;
    
    @Column(name = "CreatedAt")
    private LocalDateTime createdAt;
}
