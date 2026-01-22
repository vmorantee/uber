package com.ridesharing.admin.controller;

import com.ridesharing.admin.entity.User;
import com.ridesharing.admin.repository.UserRepository;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;

@Controller
@RequestMapping("/users")
public class UserController {
    
    private final UserRepository userRepository;
    
    public UserController(UserRepository userRepository) {
        this.userRepository = userRepository;
    }
    
    @GetMapping
    public String listUsers(Model model) {
        model.addAttribute("currentPage", "users");
        List<User> users = userRepository.findAll();
        model.addAttribute("users", users);
        return "users";
    }
    
    @PostMapping("/{id}/approve-driver")
    public String approveDriver(@PathVariable Integer id) {
        User user = userRepository.findById(id)
            .orElseThrow(() -> new RuntimeException("User not found"));
        
        user.setIsDriverApproved(true);
        user.setUpdatedAt(LocalDateTime.now());
        userRepository.save(user);
        
        return "redirect:/users";
    }
    
    @PostMapping("/{id}/revoke-driver")
    public String revokeDriver(@PathVariable Integer id) {
        User user = userRepository.findById(id)
            .orElseThrow(() -> new RuntimeException("User not found"));
        
        user.setIsDriverApproved(false);
        user.setUpdatedAt(LocalDateTime.now());
        userRepository.save(user);
        
        return "redirect:/users";
    }
}
