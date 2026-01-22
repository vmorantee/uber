package com.ridesharing.admin.controller;

import com.ridesharing.admin.entity.Ride;
import com.ridesharing.admin.repository.RideRepository;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@Controller
@RequestMapping("/rides")
public class RideController {
    
    private final RideRepository rideRepository;
    
    public RideController(RideRepository rideRepository) {
        this.rideRepository = rideRepository;
    }
    
    @GetMapping
    public String listRides(Model model) {
        model.addAttribute("currentPage", "rides");
        List<Ride> rides = rideRepository.findAll();
        
        long totalRides = rides.size();
        long inProgressCount = rides.stream()
                .filter(r -> "IN_PROGRESS".equals(r.getStatus()) || "ACCEPTED".equals(r.getStatus()))
                .count();
        long completedCount = rides.stream()
                .filter(r -> "COMPLETED".equals(r.getStatus()))
                .count();
        
        var totalRevenue = rideRepository.getTotalRevenue();
        
        model.addAttribute("rides", rides);
        model.addAttribute("totalRides", totalRides);
        model.addAttribute("inProgressCount", inProgressCount);
        model.addAttribute("completedCount", completedCount);
        model.addAttribute("totalRevenue", totalRevenue != null ? totalRevenue : java.math.BigDecimal.ZERO);
        
        return "rides";
    }
    
    @GetMapping("/{id}")
    public String viewRide(@PathVariable Integer id, Model model) {
        model.addAttribute("currentPage", "rides");
        Ride ride = rideRepository.findById(id)
            .orElseThrow(() -> new RuntimeException("Ride not found"));
        
        model.addAttribute("ride", ride);
        return "ride-detail";
    }
}
